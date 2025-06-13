using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;

internal class PluginVersionsHandler : IRequestHandler<PluginVersionsRequest, Result<IEnumerable<PluginVersionsResponse>>>
{
    private readonly ILogger<PluginVersionsHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public PluginVersionsHandler(
        ILogger<PluginVersionsHandler> logger,
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<IEnumerable<PluginVersionsResponse>>> Handle(PluginVersionsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var pluginVersions = await _pluginVersionService.GetVersionsByPluginType(request.PluginType, cancellationToken);
            if (pluginVersions is null)
                throw new Exception($"Plugin versions for '{request.PluginType}' could not be found");
            
            var response = pluginVersions.Select(version => new PluginVersionsResponse
            {
                Type = request.PluginType,
                Version = version.Version,
                IsLatest = version.IsLatest,
                Downloads = version.Statistics.Count,
                LastUpdated = version.LastModifiedOn ?? version.CreatedOn,
            }).ToList();

            return await Result<IEnumerable<PluginVersionsResponse>>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<IEnumerable<PluginVersionsResponse>>.FailAsync(ex.ToString());
        }
    }
}