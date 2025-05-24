using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;

namespace FlowSynx.Pluginregistry.Services;

public interface IStatsApiService
{
    Task<Result<IEnumerable<PluginsListResponse>>?> GetPlugins(string? q);
    Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version);
}