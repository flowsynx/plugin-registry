using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using System.Net;

namespace FlowSynx.Pluginregistry.Services;

public class StatsApiService : IStatsApiService
{
    private readonly HttpClient _http;

    public StatsApiService(IHttpClientFactory factory)
        => _http = factory.CreateClient("Api");

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
}