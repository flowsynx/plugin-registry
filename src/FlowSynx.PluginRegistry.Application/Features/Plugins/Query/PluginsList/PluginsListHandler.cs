using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

internal class PluginsListHandler : IRequestHandler<PluginsListRequest, PaginatedResult<PluginsListResponse>>
{
    private readonly ILogger<PluginsListHandler> _logger;
    private readonly IPluginService _pluginService;

    public PluginsListHandler(
        ILogger<PluginsListHandler> logger,
        IPluginService pluginService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginService);
        _logger = logger;
        _pluginService = pluginService;
    }

    public async Task<PaginatedResult<PluginsListResponse>> Handle(PluginsListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Pagination<PluginEntity> plugins;
            if (!string.IsNullOrEmpty(request.Query))
                plugins = await _pluginService.AllBySeachQuery(request.Query, request.Page ?? 1, cancellationToken);
            else if (!string.IsNullOrEmpty(request.Tag))
                plugins = await _pluginService.AllBySeachTags(request.Tag, request.Page ?? 1, cancellationToken);
            else
                plugins = await _pluginService.All(request.Page ?? 1, cancellationToken);
            
            var response = plugins.Data.Select(p => new PluginsListResponse
            {
                Type = p.Type,
                Version = p.LatestVersion!.Version,
                Owners = p.Owners.Select(x=>x.Profile!.UserName),
                Description = p.LatestVersion!.Description,
                CategoryTitle = p.LatestVersion.PluginCategory.Title,
                LastUpdated = p.LastModifiedOn ?? p.CreatedOn,
                Tags = p.LatestVersion!.PluginVersionTags.Select(x => x.Tag!.Name),
                TotalDownload = p.Versions.Sum(x=>x.Statistics.Count)
            }).ToList();

            return PaginatedResult<PluginsListResponse>.Success(response, plugins.TotalCount, request.Page ?? 1, plugins.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return PaginatedResult<PluginsListResponse>.Failure(ex.ToString());
        }
    }
}