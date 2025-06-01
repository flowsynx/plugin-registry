using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginIcon;

internal class PluginIconHandler : IRequestHandler<PluginIconRequest, Result<PluginIconResponse>>
{
    private readonly ILogger<PluginIconHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public PluginIconHandler(
        ILogger<PluginIconHandler> logger,
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<PluginIconResponse>> Handle(PluginIconRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginVersionService.GetByPluginType(request.PluginType, request.PluginVersion, cancellationToken);
            if (plugin is null)
                throw new Exception($"Plugin details for '{request.PluginType}' v{request.PluginVersion} could not be found");
            
            var response = new PluginIconResponse
            {
                Icon = plugin.Icon,
            };

            return await Result<PluginIconResponse>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<PluginIconResponse>.FailAsync(ex.ToString());
        }
    }
}