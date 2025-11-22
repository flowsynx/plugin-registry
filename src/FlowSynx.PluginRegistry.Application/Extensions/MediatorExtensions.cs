using FlowSynx.PluginRegistry.Application.Features.Plugins.Command.SetPluginVersionActiveStatus;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginIcon;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginLocation;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginReadme;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;
using FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;
using FlowSynx.PluginRegistry.Application.Features.Statistics.Command.AddStatistic;
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
        this IMediator mediator, string pluginType, string pluginVersion, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginDetailsRequest 
        { 
            PluginType = pluginType, 
            PluginVersion = pluginVersion 
        }, cancellationToken);
    }

    public static Task<Result<IEnumerable<PluginVersionsResponse>>> PluginVersions(
        this IMediator mediator, string pluginType, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginVersionsRequest
        {
            PluginType = pluginType
        }, cancellationToken);
    }

    public static Task<Result<PluginIconResponse>> PluginIcon(
        this IMediator mediator, string pluginType, string pluginVersion, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginIconRequest
        {
            PluginType = pluginType,
            PluginVersion = pluginVersion
        }, cancellationToken);
    }

    public static Task<Result<PluginReadmeResponse>> PluginReadme(
        this IMediator mediator, string pluginType, string pluginVersion, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginReadmeRequest
        {
            PluginType = pluginType,
            PluginVersion = pluginVersion
        }, cancellationToken);
    }

    public static Task<PaginatedResult<PluginsListByProfileResponse>> PluginsListByuserName(
        this IMediator mediator, string userName, int? page, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginsListByProfileRequest { UserName = userName, Page = page }, cancellationToken);
    }

    public static Task<Result<PluginLocationResponse>> PluginLocation(
        this IMediator mediator, string pluginType, string pluginVersion, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginLocationRequest
        {
            PluginType = pluginType,
            PluginVersion = pluginVersion
        }, cancellationToken);
    }

    public static Task<Result<bool>> EnablePluginVersion(
        this IMediator mediator, 
        string pluginType, 
        string pluginVersion, 
        CancellationToken cancellationToken)
    {
        return mediator.Send(new SetPluginVersionActiveStatusRequest
        {
            PluginType = pluginType,
            PluginVersion = pluginVersion,
            IsActive = true
        }, cancellationToken);
    }

    public static Task<Result<bool>> DisablePluginVersion(
        this IMediator mediator, 
        string pluginType, 
        string pluginVersion, 
        CancellationToken cancellationToken)
    {
        return mediator.Send(new SetPluginVersionActiveStatusRequest
        {
            PluginType = pluginType,
            PluginVersion = pluginVersion,
            IsActive = false
        }, cancellationToken);
    }
    #endregion

    #region Profile
    public static Task<Result<PluginsStatisticsByProfileResponse>> PluginStatisticsByUsernameAsync(
        this IMediator mediator, string username, CancellationToken cancellationToken)
    {
        return mediator.Send(new PluginsStatisticsByProfileRequest { UserName = username }, cancellationToken);
    }
    #endregion

    #region Statistics
    public static Task<Result<Unit>> IncreaseDownloadCountAsync(
        this IMediator mediator, AddStatisticRequest request, CancellationToken cancellationToken)
    {
        return mediator.Send(request, cancellationToken);
    }
    #endregion
}