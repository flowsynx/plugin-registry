using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Extensions;

public static class MediatorExtensions
{
    #region Plugins
    public static Task<PaginatedResult<PluginsListResponse>> PluginsList(
        this IMediator mediator, string? query, int? page, CancellationToken cancellationToken)
    {
        var queryValue = query;
        var tagValue = "";

        if (!string.IsNullOrEmpty(query))
        {
            var qText = query.ToString();
            if (qText.StartsWith("tag:"))
            {
                tagValue = qText.Substring(4);
                queryValue = "";
            }
        }

        return mediator.Send(new PluginsListRequest { Query = queryValue, Tag = tagValue, Page = page }, cancellationToken);
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