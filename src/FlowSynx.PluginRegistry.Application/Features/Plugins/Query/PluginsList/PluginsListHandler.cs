using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

internal class PluginsListHandler : IRequestHandler<PluginsListRequest, Result<IEnumerable<PluginsListResponse>>>
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

    public async Task<Result<IEnumerable<PluginsListResponse>>> Handle(PluginsListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var plugins = await _pluginService.AllBySeachQuery(request.Query, cancellationToken);
            var response = plugins.Select(p => new PluginsListResponse
            {
                Type = p.Type,
                Version = p.LatestVersion,
                Owners = p.Owners.Select(x=>x.Profile.UserName),
                Description = p.LatestDescription,
                LastUpdated = p.LastModifiedOn ?? p.CreatedOn,
                Tags = p.LatestTags
            });
            return await Result<IEnumerable<PluginsListResponse>>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<IEnumerable<PluginsListResponse>>.FailAsync(ex.ToString());
        }
    }
}