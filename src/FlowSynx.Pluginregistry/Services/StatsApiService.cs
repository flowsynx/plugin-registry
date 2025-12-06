using FlowSynx.Pluginregistry.Models;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;
using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Profile;
using FlowSynx.PluginRegistry.Domain.ApiKey;
using Microsoft.AspNetCore.Components.Forms;
using System.IO.Compression;

namespace FlowSynx.Pluginregistry.Services;

public class StatsApiService : IStatsApiService
{
    private readonly IApiClient _apiClient;
    private readonly IWebHostEnvironment _environment;
    private readonly IFileStorage _fileStorage;
    private readonly IPluginVersionService _pluginVersionService;
    private readonly IPluginService _pluginService;
    private readonly IFileValidator _fileValidator;
    private readonly IPluginMetadataReader _metadataReader;
    private readonly IPluginCategoryService _pluginCategoryService;
    private readonly IApiKeyService _apiKeyService;
    private const int MaxUploadSize = 100 * 1024 * 1024; // 100MB

    public StatsApiService(
        IApiClient apiClient,
        IWebHostEnvironment environment,
        IFileStorage fileStorage,
        IPluginVersionService pluginVersionService,
        IPluginService pluginService,
        IFileValidator fileValidator,
        IPluginMetadataReader metadataReader,
        IPluginCategoryService pluginCategoryService,
        IApiKeyService apiKeyService)
    {
        _apiClient = apiClient;
        _environment = environment;
        _fileStorage = fileStorage;
        _pluginVersionService = pluginVersionService;
        _pluginService = pluginService;
        _fileValidator = fileValidator;
        _metadataReader = metadataReader;
        _pluginCategoryService = pluginCategoryService;
        _apiKeyService = apiKeyService;
    }

    public Task<PaginatedResult<PluginsListResponse>?> GetPlugins(string? query, int? page)
    {
        var url = string.IsNullOrEmpty(query)
            ? $"/api/plugins?page={page}"
            : $"/api/plugins?page={page}&q={query}";
        return _apiClient.GetAsync<PaginatedResult<PluginsListResponse>>(url);
    }

    public Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version)
    {
        var url = $"/api/plugins/{type}/{version}";
        return _apiClient.GetAsync<Result<PluginDetailsResponse>>(url);
    }

    public async Task<Result<bool>> EnablePluginVersion(string pluginType, string version)
    {
        try
        {
            var url = $"/api/plugins/{pluginType}/{version}/enable";
            var response = await _apiClient.PutAsync<Result<bool>, bool>(url, true, CancellationToken.None);

            if (response != null)
            {
                return response ?? await Result<bool>.SuccessAsync(true);
            }

            return await Result<bool>.FailAsync("Failed to enable plugin version");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync($"Error: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DisablePluginVersion(string pluginType, string version)
    {
        try
        {
            var url = $"/api/plugins/{pluginType}/{version}/disable";
            var response = await _apiClient.PutAsync<Result<bool>, bool>(url, false, CancellationToken.None);

            if (response != null)
            {
                return response ?? await Result<bool>.SuccessAsync(true);
            }

            return await Result<bool>.FailAsync("Failed to enable plugin version");
        }
        catch (Exception ex)
        {
            return await Result<bool>.FailAsync($"Error: {ex.Message}");
        }
    }

    public Task<Result<IEnumerable<PluginVersionsResponse>>?> GetPluginVersions(string? type)
    {
        var url = $"/api/plugins/{type}/versions";
        return _apiClient.GetAsync<Result<IEnumerable<PluginVersionsResponse>>>(url);
    }

    public Task<PaginatedResult<PluginsListByProfileResponse>?> GetPluginsByUserName(string userName, int? page)
    {
        var url = page is int and > 1
            ? $"/api/profiles/{userName}?page={page}"
            : $"/api/profiles/{userName}";
        return _apiClient.GetAsync<PaginatedResult<PluginsListByProfileResponse>>(url);
    }

    public Task<Result<PluginsStatisticsByProfileResponse>?> GetPluginStatisticsByUsername(string? userName)
    {
        var url = $"/api/profiles/{userName}/statistics";
        return _apiClient.GetAsync<Result<PluginsStatisticsByProfileResponse>>(url);
    }

    public async Task UploadFileAsync(
        IBrowserFile file,
        Func<int, Task> onProgress,
        Guid profileId,
        string? apiKey = null,
        CancellationToken cancellationToken = default)
    {
        _fileValidator.Validate(file);
        var tempPath = CreateTempDirectory();

        try
        {
            var filePath = await SaveTempFileAsync(file, tempPath, onProgress, cancellationToken);
            ZipFile.ExtractToDirectory(filePath, tempPath);

            _fileValidator.ValidatePackageContents(tempPath, out var pluginFile, out var expectedHash);
            var checksum = await _fileValidator.ValidateChecksumAsync(pluginFile, expectedHash, cancellationToken);
            var metadata = await _metadataReader.ReadAsync(tempPath);

            // Validate API key if provided
            if (!string.IsNullOrEmpty(apiKey))
            {
                await ValidateApiKeyForUpload(apiKey, metadata.Type, profileId, cancellationToken);
            }

            var extractedPluginPath = ExtractPlugin(pluginFile, tempPath);
            var destPath = Path.Combine(metadata.Type, metadata.Version);

            var destinationIconPath = await HandleAssetAsync(metadata.Icon, extractedPluginPath, destPath, false);
            var destinationReadMePath = await HandleAssetAsync(metadata.ReadMe, extractedPluginPath, destPath, false);
            var destinationMetadataPath = await HandleAssetAsync(Path.Combine(tempPath, "metadata.json"), extractedPluginPath, destPath, true);

            await SavePluginAsync(metadata, filePath, destPath, destinationIconPath, destinationReadMePath, 
                                  destinationMetadataPath, profileId, checksum, cancellationToken);
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

    private async Task ValidateApiKeyForUpload(string apiKey, string pluginType, Guid profileId, CancellationToken cancellationToken)
    {
        var pluginEntity = await _pluginService.GetByPluginType(pluginType, cancellationToken);
        var isNewPlugin = pluginEntity == null;
        var pluginId = pluginEntity?.Id;

        var canPush = await _apiKeyService.CanPushAsync(apiKey, pluginId, profileId, isNewPlugin, cancellationToken);
        
        if (!canPush)
        {
            throw new UnauthorizedAccessException(
                isNewPlugin 
                    ? "API key does not have permission to push new plugins." 
                    : "API key does not have permission to push plugin versions for this plugin.");
        }
    }

    private static string ExtractPlugin(string pluginFile, string tempPath)
    {
        var extractedPath = Path.Combine(tempPath, Guid.NewGuid().ToString());
        ZipFile.ExtractToDirectory(pluginFile, extractedPath);
        return extractedPath;
    }

    private async Task SavePluginAsync(
        PluginMetadata metadata,
        string filePath,
        string destPath,
        string iconPath,
        string readMePath,
        string metadataPath,
        Guid profileId,
        string checksum,
        CancellationToken cancellationToken)
    {
        var pluginEntity = await _pluginService.GetByPluginType(metadata.Type, cancellationToken);
        bool isNewPlugin = pluginEntity == null;

        if (pluginEntity?.Versions.Any(v => v.Version == metadata.Version) == true)
            throw new Exception($"The plugin '{metadata.Type}' v{metadata.Version} already exists.");

        var pluginCategoryEntity = await _pluginCategoryService.GetByCategoryId(metadata.CategoryId, cancellationToken);
        if (pluginCategoryEntity == null)
            throw new Exception($"The plugin category '{metadata.CategoryId}' is not valid.");

        var savedPath = await SaveFinalPluginFile(filePath, destPath, metadata);

        if (isNewPlugin)
        {
            await AddNewPluginAsync(metadata, iconPath, readMePath, metadataPath, savedPath,
                profileId, pluginCategoryEntity.Id, checksum, cancellationToken);
        }
        else
        {
            await AddPluginVersionAsync(pluginEntity!, metadata, iconPath, readMePath, metadataPath,
                savedPath, pluginCategoryEntity.Id, checksum, cancellationToken);
        }
    }

    private async Task<string> SaveFinalPluginFile(string sourcePath, string destinationPath, PluginMetadata metadata)
    {
        var destinationFile = Path.Combine(destinationPath, Path.GetFileName(sourcePath));
        var content = await File.ReadAllBytesAsync(sourcePath);
        await _fileStorage.SaveFileAsync(destinationFile, content);
        return destinationFile;
    }

    private async Task AddNewPluginAsync(
        PluginMetadata metadata,
        string destinationIconPath,
        string destinationReadMePath,
        string destinationMetadataPath, 
        string savedPath, 
        Guid profileId, 
        Guid pluginCategoryId, 
        string checksum, 
        CancellationToken cancellationToken)
    {
        var pluginId = Guid.NewGuid();
        var versionId = Guid.NewGuid();

        var version = CreatePluginVersionEntity(versionId, pluginId, metadata, destinationIconPath,
            destinationReadMePath, destinationMetadataPath, savedPath, pluginCategoryId, checksum);
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
        string destinationIconPath,
        string destinationReadMePath,
        string destinationMetadataPath,
        string savedPath,
        Guid pluginCategoryId,
        string checksum,
        CancellationToken cancellationToken)
    {
        if (!Version.TryParse(metadata.Version, out var publishingVersion))
        {
            throw new InvalidOperationException($"Invalid version format: {metadata.Version}");
        }

        var latestVersionEntity = plugin.Versions
            .OrderByDescending(v => Version.TryParse(v.Version, out var ver) ? ver : new Version(0, 0))
            .FirstOrDefault();

        if (latestVersionEntity != null &&
            Version.TryParse(latestVersionEntity.Version, out var latestVersion) &&
            publishingVersion <= latestVersion)
        {
            throw new Exception($"The publishing plugin version must be greater than the current latest version. " +
                $"You try to upload plugin version '{metadata.Version}', " +
                $"while the current latest plugin version is '{latestVersionEntity.Version}'");
        }

        // Mark all existing versions as not latest
        foreach (var pluginVersionEntity in plugin.Versions)
        {
            pluginVersionEntity.IsLatest = false;
            await _pluginVersionService.Update(pluginVersionEntity, cancellationToken);
        }

        var version = CreatePluginVersionEntity(Guid.NewGuid(), plugin.Id, metadata, destinationIconPath,
            destinationReadMePath, destinationMetadataPath, savedPath, pluginCategoryId, checksum);

        await _pluginVersionService.Add(version, cancellationToken);

        plugin.LatestVersionId = version.Id;
        await _pluginService.Update(plugin, cancellationToken);

        await _pluginVersionService.AddTagsToPluginVersionAsync(version.Id, metadata.Tags, cancellationToken);
    }

    private PluginVersionEntity CreatePluginVersionEntity(Guid id, Guid pluginId, PluginMetadata metadata,
        string destinationIconPath, string destinationReadMePath, string destinationMetadataPath,
        string path, Guid pluginCategoryId, string checksum)
    {
        return new PluginVersionEntity
        {
            Id = id,
            Version = metadata.Version,
            PluginId = pluginId,
            PluginLocation = path,
            Description = metadata.Description,
            Icon = destinationIconPath,
            Authors = metadata.Authors,
            Copyright = metadata.Copyright,
            License = metadata.License,
            LicenseUrl = metadata.LicenseUrl,
            ProjectUrl = metadata.ProjectUrl,
            RepositoryUrl = metadata.RepositoryUrl,
            ReadMe = destinationReadMePath,
            PluginCategoryId = pluginCategoryId,
            MinimumFlowSynxVersion = metadata.MinimumFlowSynxVersion,
            TargetFlowSynxVersion = metadata.TargetFlowSynxVersion,
            Specifications = System.Text.Json.JsonSerializer.Serialize(metadata.Specifications),
            Operations = System.Text.Json.JsonSerializer.Serialize(metadata.Operations),
            IsLatest = true,
            MetadataFile = destinationMetadataPath,
            Checksum = checksum,
            IsDeleted = false
        };
    }

    private async Task<string> HandleAssetAsync(string? assetPath, string extractedPath, string destPath, bool errorIfSourcePathNotFound = false)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            return string.Empty;

        string sourcePath = assetPath;
        string fileName;

        if (!IsHttpPath(assetPath))
        {
            sourcePath = Path.Combine(extractedPath, assetPath);
            fileName = Path.GetFileName(sourcePath);
        }
        else
        {
            fileName = Path.GetFileName(new Uri(assetPath).AbsolutePath);
        }

        var destinationPath = Path.Combine(destPath, fileName);
        var isFileDownloaded = await DownloadFileAsync(sourcePath, destinationPath, errorIfSourcePathNotFound);
        return isFileDownloaded ? destinationPath : "";
    }

    private bool IsHttpPath(string path) => Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.Scheme.StartsWith("http");

    private async Task<bool> DownloadFileAsync(string sourcePathOrUrl, string destinationPath, bool errorIfSourcePathNotFound = false)
    {
        if (string.IsNullOrWhiteSpace(sourcePathOrUrl))
            throw new ArgumentException("Source path or URL cannot be null or empty.", nameof(sourcePathOrUrl));

        if (string.IsNullOrWhiteSpace(destinationPath))
            throw new ArgumentException("Destination path cannot be null or empty.", nameof(destinationPath));

        if (IsHttpPath(sourcePathOrUrl))
        {
            var content = await _apiClient.DownloadAsync(sourcePathOrUrl);
            await _fileStorage.SaveFileAsync(destinationPath, content);
            return true;
        }
        else
        {
            if (!File.Exists(sourcePathOrUrl))
            {
                if (errorIfSourcePathNotFound)
                    throw new FileNotFoundException("Local source file not found.", sourcePathOrUrl);

                return false;
            }

            var content = await File.ReadAllBytesAsync(sourcePathOrUrl);
            await _fileStorage.SaveFileAsync(destinationPath, content);
            return true;
        }
    }

    private string CreateTempDirectory()
    {
        var tempPath = Path.Combine(_environment.ContentRootPath, "temp", Path.GetRandomFileName());
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

    private void CleanupTempDirectory(string tempPath)
    {
        try
        {
            Directory.Delete(tempPath, true);
        }
        catch
        {
            // log if necessary
        }
    }
}