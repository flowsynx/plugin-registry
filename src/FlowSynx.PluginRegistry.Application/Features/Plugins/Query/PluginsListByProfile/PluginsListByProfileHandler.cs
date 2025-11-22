using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;

internal class PluginsListByProfileHandler : IRequestHandler<PluginsListByProfileRequest, PaginatedResult<PluginsListByProfileResponse>>
{
    private readonly ILogger<PluginsListByProfileHandler> _logger;
    private readonly IPluginService _pluginService;

    public PluginsListByProfileHandler(
        ILogger<PluginsListByProfileHandler> logger,
        IPluginService pluginService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginService);
        _logger = logger;
        _pluginService = pluginService;
    }

    public async Task<PaginatedResult<PluginsListByProfileResponse>> Handle(PluginsListByProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Pagination<PluginEntity> plugins = await _pluginService.AllByProfileUserName(
                request.UserName, 
                request.Page ?? 1, 
                cancellationToken
            );

            var response = plugins.Data.Select(p => new PluginsListByProfileResponse
            {
                Id = p.Id,
                Type = p.Type,
                Version = p.LatestVersion!.Version,
                Owners = p.Owners.Select(x => x.Profile!.UserName),
                CategoryTitle = p.LatestVersion.PluginCategory.Title,
                Description = p.LatestVersion!.Description,
                LastUpdated = p.LastModifiedOn ?? p.CreatedOn,
                Tags = p.LatestVersion!.PluginVersionTags.Select(x => x.Tag!.Name),
                TotalDownload = p.Versions.Sum(x => x.Statistics.Count),
                IsTrusted = p.IsTrusted
            }).ToList();

            return PaginatedResult<PluginsListByProfileResponse>.Success(response, plugins.TotalCount, request.Page ?? 1, plugins.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return PaginatedResult<PluginsListByProfileResponse>.Failure(ex.ToString());
        }
    }
}