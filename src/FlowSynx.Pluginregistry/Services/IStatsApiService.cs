using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using Microsoft.AspNetCore.Components.Forms;

namespace FlowSynx.Pluginregistry.Services;

public interface IStatsApiService
{
    Task<PaginatedResult<PluginsListResponse>?> GetPlugins(string? query, int? page);
    Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version);
    Task UploadFileAsync(IBrowserFile file, Func<int, Task> onProgress, 
        Guid profileId, CancellationToken cancellationToken = default);
}