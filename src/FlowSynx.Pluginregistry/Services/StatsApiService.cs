using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;

namespace FlowSynx.Pluginregistry.Services;

public class StatsApiService : IStatsApiService
{
    private readonly HttpClient _http;
    private readonly IWebHostEnvironment _env;

    public StatsApiService(IHttpClientFactory factory, IWebHostEnvironment env)
    {
        _http = factory.CreateClient("Api");
        _env = env;
    }
    public async Task<PaginatedResult<PluginsListResponse>?> GetPlugins(string? query, int? page)
    {
        try
        {
            HttpResponseMessage? response;
            if (!string.IsNullOrEmpty(query))
                response = await _http.GetAsync($"/api/plugins?page={page}&q={query}");
            else
                response = await _http.GetAsync($"/api/plugins?page={page}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult < PluginsListResponse>>();
            }
            else
            {
                return PaginatedResult<PluginsListResponse>.Failure($"API call failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return PaginatedResult<PluginsListResponse>.Failure(ex.Message);
        }
    }

    public async Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version)
    {
        try
        {
            var response = await _http.GetAsync($"/api/plugins/{type}/{version}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Result<PluginDetailsResponse>>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return await Result<PluginDetailsResponse>.FailAsync($"Plugin '{type}' v{version} not found");
            }
            else
            {
                return await Result<PluginDetailsResponse>.FailAsync($"API call failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return await Result<PluginDetailsResponse>.FailAsync(ex.Message);
        }
    }

    public async Task<string> UploadFileAsync(
            IBrowserFile file,
            Func<int, Task> onProgress,
            CancellationToken cancellationToken = default)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        var allowedExtensions = new[] { ".fsp" };
        var extension = Path.GetExtension(file.Name)?.ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new Exception("Invalid file type. Only '.fsp' file is allowed.");

        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        var fileName = Path.GetFileName(file.Name);
        var filePath = Path.Combine(uploadPath, fileName);

        const int bufferSize = 81920;
        var totalBytes = file.Size;
        long totalRead = 0;

        await using var fileStream = File.Create(filePath);
        await using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
        var buffer = new byte[bufferSize];

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalRead += bytesRead;

            var percent = (int)((totalRead * 100) / totalBytes);
            await onProgress(percent);
        }

        return filePath;
    }
}