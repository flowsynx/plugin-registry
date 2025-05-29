using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginLocation;

internal class PluginLocationHandler : IRequestHandler<PluginLocationRequest, Result<PluginLocationResponse>>
{
    private readonly ILogger<PluginLocationHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public PluginLocationHandler(
        ILogger<PluginLocationHandler> logger,
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<PluginLocationResponse>> Handle(PluginLocationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginVersionService.GetByPluginType(request.PluginType, request.PluginVersion, cancellationToken);
            if (plugin is null)
                throw new Exception($"Plugin details for '{request.PluginType}' v{request.PluginVersion} could not be found");
            
            var response = new PluginLocationResponse
            {
                PluginLocation = plugin.PluginLocation,
                Checksum = plugin.Checksum
            };

            return await Result<PluginLocationResponse>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<PluginLocationResponse>.FailAsync(ex.ToString());
        }
    }
}