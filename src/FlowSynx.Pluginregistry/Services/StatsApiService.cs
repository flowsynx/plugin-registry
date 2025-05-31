using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;
using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Profile;
using Microsoft.AspNetCore.Components.Forms;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace FlowSynx.Pluginregistry.Services;

public class StatsApiService : IStatsApiService
{
    private readonly HttpClient _http;
    private readonly IWebHostEnvironment _env;
    private readonly IPluginVersionService _pluginVersionService;
    private readonly IPluginService _pluginService;
    private const int MaxUploadSize = 100 * 1024 * 1024; // 100MB
    private static readonly string[] AllowedExtensions = { ".fspack" };

    public StatsApiService(
        IHttpClientFactory factory, 
        IWebHostEnvironment env,
        IPluginVersionService pluginVersionService,
        IPluginService pluginService)
    {
        _http = factory.CreateClient("Api");
        _env = env;
        _pluginVersionService = pluginVersionService;
        _pluginService = pluginService;
    }

    public async Task<PaginatedResult<PluginsListResponse>?> GetPlugins(string? query, int? page)
    {
        var url = string.IsNullOrEmpty(query)
            ? $"/api/plugins?page={page}"
            : $"/api/plugins?page={page}&q={query}";

        return await SendGetRequest<PaginatedResult<PluginsListResponse>>(url);
    }

    public async Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version)
    {
        var url = $"/api/plugins/{type}/{version}";
        try
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.StatusCode == HttpStatusCode.NotFound
                ? $"Plugin '{type}' v{version} not found"
                : $"API call failed with status code: {response.StatusCode}";

                return await Result<PluginDetailsResponse>.FailAsync(error);
            }

            return await response.Content.ReadFromJsonAsync<Result<PluginDetailsResponse>>();
        }
        catch (Exception ex)
        {
            return await Result<PluginDetailsResponse>.FailAsync(ex.Message);
        }
    }

    public async Task<PaginatedResult<PluginsListByProfileResponse>?> GetPluginsByUserName(string userName, int? page)
    {
        var url = page is int && page > 1
            ? $"/api/profiles/{userName}?page={page}"
            : $"/api/profiles/{userName}";

        return await SendGetRequest<PaginatedResult<PluginsListByProfileResponse>>(url);
    }

    public async Task<Result<PluginsStatisticsByProfileResponse>?> GetPluginStatisticsByUsername(string? userName)
    {
        var url = $"/api/profiles/{userName}/statistics";
        try
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = response.StatusCode == HttpStatusCode.NotFound
                ? $"Profile '{userName}' not found"
                : $"API call failed with status code: {response.StatusCode}";

                return await Result<PluginsStatisticsByProfileResponse>.FailAsync(error);
            }

            return await response.Content.ReadFromJsonAsync<Result<PluginsStatisticsByProfileResponse>>();
        }
        catch (Exception ex)
        {
            return await Result<PluginsStatisticsByProfileResponse>.FailAsync(ex.Message);
        }
    }

    public async Task UploadFileAsync(
        IBrowserFile file,
        Func<int, Task> onProgress,
        Guid profileId,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);

        var tempPath = CreateTempDirectory();
        var filePath = await SaveTempFileAsync(file, tempPath, onProgress, cancellationToken);

        try
        {
            var extractedPath = tempPath;
            ZipFile.ExtractToDirectory(filePath, extractedPath);

            ValidatePackageContents(extractedPath, out var pluginFile, out var expectedHash);
            var checksum = await ValidateChecksumAsync(pluginFile, expectedHash, cancellationToken);
            var metadata = await ReadPluginMetadataAsync(extractedPath);

            var pluginEntity = await _pluginService.GetByPluginType(metadata.Type, cancellationToken);
            var isNewPlugin = pluginEntity == null;

            if (pluginEntity?.Versions.Any(v => v.Version == metadata.Version) == true)
                throw new Exception($"The plugin '{metadata.Type}' v{metadata.Version} already exists.");

            var savedPath = SaveFinalPluginFile(filePath, metadata);

            if (isNewPlugin)
                await AddNewPluginAsync(metadata, savedPath, profileId, checksum, cancellationToken);
            else
                await AddPluginVersionAsync(pluginEntity!, metadata, savedPath, checksum, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Upload failed: {ex.Message}");
        }
        finally
        {
            CleanupTempDirectory(tempPath);
        }
    }

    private async Task<T?> SendGetRequest<T>(string url)
    {
        try
        {
            var response = await _http.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>()
                : default;
        }
        catch
        {
            return default;
        }
    }

    private static void ValidateFile(IBrowserFile file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        var extension = Path.GetExtension(file.Name)?.ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            throw new Exception("Invalid file type. Only '.fspack' files are allowed.");
    }

    private string CreateTempDirectory()
    {
        var tempPath = Path.Combine(_env.ContentRootPath, "temp", Path.GetRandomFileName());
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    private async Task<string> SaveTempFileAsync(
        IBrowserFile file,
        string tempPath,
        Func<int, Task> onProgress,
        CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(tempPath, Path.GetFileName(file.Name));

        await using var fileStream = File.Create(filePath);
        await using var inputStream = file.OpenReadStream(MaxUploadSize);

        var buffer = new byte[81920];
        long totalRead = 0;

        while (true)
        {
            var read = await inputStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            if (read == 0) break;

            await fileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            totalRead += read;
            await onProgress((int)(totalRead * 100 / file.Size));
        }

        return filePath;
    }

    private void ValidatePackageContents(string path, out string pluginFile, out string expectedHash)
    {
        expectedHash = File.ReadAllText(Directory.GetFiles(path, "*.sha256").FirstOrDefault()
                                        ?? throw new Exception(".sha256 checksum file not found.")).Trim();

        pluginFile = Directory.GetFiles(path, "*.plugin").FirstOrDefault()
                     ?? throw new Exception(".plugin file not found in the package.");
    }

    private async Task<string> ValidateChecksumAsync(string filePath, string expectedHash, CancellationToken cancellationToken)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);
        var actualHash = Convert.ToHexString(await sha256.ComputeHashAsync(stream, cancellationToken)).ToLowerInvariant();

        if (!expectedHash.Equals(actualHash, StringComparison.OrdinalIgnoreCase))
            throw new Exception("Checksum validation failed.");

        return actualHash;
    }

    private async Task<PluginMetadata> ReadPluginMetadataAsync(string path)
    {
        var manifestPath = Path.Combine(path, "manifest.json");

        if (!File.Exists(manifestPath))
            throw new Exception("manifest.json not found.");

        var content = await File.ReadAllTextAsync(manifestPath);
        var metadata = JsonSerializer.Deserialize<PluginMetadata>(content);

        if (metadata == null || string.IsNullOrWhiteSpace(metadata.Type) || string.IsNullOrWhiteSpace(metadata.Version))
            throw new Exception("Invalid or incomplete manifest.json.");

        return metadata;
    }

    private string SaveFinalPluginFile(string sourcePath, PluginMetadata metadata)
    {
        var destPath = Path.Combine(_env.ContentRootPath, "plugins", metadata.Type, metadata.Version);
        Directory.CreateDirectory(destPath);

        var destinationFile = Path.Combine(destPath, Path.GetFileName(sourcePath));
        File.Copy(sourcePath, destinationFile, true);
        return destinationFile;
    }

    private async Task AddNewPluginAsync(PluginMetadata metadata, string savedPath, 
        Guid profileId, string checksum, CancellationToken cancellationToken)
    {
        var pluginId = Guid.NewGuid();
        var versionId = Guid.NewGuid();

        var version = CreatePluginVersionEntity(versionId, pluginId, metadata, checksum, savedPath);
        var plugin = new PluginEntity
        {
            Id = pluginId,
            Type = metadata.Type,
            Versions = new List<PluginVersionEntity> { version },
            Owners = new List<ProfilePluginOwnerEntity>
            {
                new ProfilePluginOwnerEntity
                {
                    PluginId = pluginId,
                    ProfileId = profileId,
                }
            }
        };

        await _pluginService.Add(plugin, cancellationToken);

        plugin.LatestVersionId = versionId;
        await _pluginService.Update(plugin, cancellationToken);

        await _pluginVersionService.AddTagsToPluginVersionAsync(versionId, metadata.Tags, cancellationToken);
    }

    private async Task AddPluginVersionAsync(
        PluginEntity plugin,
        PluginMetadata metadata,
        string checksum,
        string savedPath,
        CancellationToken cancellationToken)
    {
        var version = CreatePluginVersionEntity(Guid.NewGuid(), plugin.Id, metadata, checksum, savedPath);

        await _pluginVersionService.Add(version, cancellationToken);

        plugin.LatestVersionId = version.Id;
        await _pluginService.Update(plugin, cancellationToken);

        await _pluginVersionService.AddTagsToPluginVersionAsync(version.Id, metadata.Tags, cancellationToken);
    }

    private PluginVersionEntity CreatePluginVersionEntity(Guid id, Guid pluginId, PluginMetadata metadata, 
        string checksum, string path)
    {
        return new PluginVersionEntity
        {
            Id = id,
            Version = metadata.Version,
            PluginId = pluginId,
            PluginLocation = path,
            Description = metadata.Description,
            Icon = metadata.Icon,
            Authors = metadata.Authors,
            Copyright = metadata.Copyright,
            License = metadata.License,
            LicenseUrl = metadata.LicenseUrl,
            ProjectUrl = metadata.ProjectUrl,
            RepositoryUrl = metadata.RepositoryUrl,
            IsLatest = true,
            ManifestJson = JsonSerializer.Serialize(metadata),
            Checksum = checksum,
            IsDeleted = false
        };
    }

    private void CleanupTempDirectory(string tempPath)
    {
        try { Directory.Delete(tempPath, true); } catch { }
    }

    // ------------------ Nested Models ------------------

    public class PluginMetadata
    {
        public required Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Version { get; set; }
        public required string CompanyName { get; set; }
        public string? Description { get; set; }
        public string? License { get; set; }
        public string? LicenseUrl { get; set; }
        public string? Icon { get; set; }
        public string? ProjectUrl { get; set; }
        public string? RepositoryUrl { get; set; }
        public string? Copyright { get; set; }
        public List<string> Authors { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }
}