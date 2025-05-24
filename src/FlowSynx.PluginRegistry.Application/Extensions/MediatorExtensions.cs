using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Extensions;

public static class MediatorExtensions
{
    #region Plugins
    public static Task<Result<IEnumerable<PluginsListResponse>>> PluginsList(
        this IMediator mediator, string? query, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginsListRequest { Query = query }, cancellationToken);
    }

    public static Task<Result<PluginDetailsResponse>> PluginDetails(
        this IMediator mediator, string? pluginType, string pluginVersion, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginDetailsRequest 
        { 
            PluginType = pluginType, 
            PluginVersion = pluginVersion 
        }, cancellationToken);
    }
    #endregion
}