using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;
using FlowSynx.PluginRegistry.Application.Wrapper;
using Microsoft.AspNetCore.Components.Forms;

namespace FlowSynx.Pluginregistry.Services;

public interface IStatsApiService
{
    Task<PaginatedResult<PluginsListResponse>?> GetPlugins(string? query, int? page);
    Task<Result<PluginDetailsResponse>?> GetPluginDetails(string? type, string version);
    Task<Result<IEnumerable<PluginVersionsResponse>>?> GetPluginVersions(string? type);
    Task<PaginatedResult<PluginsListByProfileResponse>?> GetPluginsByUserName(string userName, int? page);
    Task<Result<PluginsStatisticsByProfileResponse>?> GetPluginStatisticsByUsername(string? userName);
    Task UploadFileAsync(IBrowserFile file, Func<int, Task> onProgress, 
        Guid profileId, CancellationToken cancellationToken = default);
    Task<Result<bool>> EnablePluginVersion(string pluginType, string version);
    Task<Result<bool>> DisablePluginVersion(string pluginType, string version);
}