using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using System;
using System.Collections.Generic;
using System.Net;

namespace FlowSynx.Pluginregistry.Services;

public class StatsApiService : IStatsApiService
{
    private readonly HttpClient _http;

    public StatsApiService(IHttpClientFactory factory)
        => _http = factory.CreateClient("Api");

    public async Task<Result<IEnumerable<PluginsListResponse>>?> GetPlugins(string? q)
    {
        try
        {
            var response = await _http.GetAsync($"/api/plugins?q={q}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Result<IEnumerable<PluginsListResponse>>>();
            }
            else
            {
                return await Result<IEnumerable<PluginsListResponse>>.FailAsync($"API call failed with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return await Result<IEnumerable<PluginsListResponse>>.FailAsync(ex.Message);
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