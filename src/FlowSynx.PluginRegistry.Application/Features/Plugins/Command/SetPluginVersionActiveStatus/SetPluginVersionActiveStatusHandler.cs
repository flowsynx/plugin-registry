using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Command.SetPluginVersionActiveStatus;

internal class SetPluginVersionActiveStatusHandler : IRequestHandler<SetPluginVersionActiveStatusRequest, Result<bool>>
{
    private readonly ILogger<SetPluginVersionActiveStatusHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public SetPluginVersionActiveStatusHandler(
        ILogger<SetPluginVersionActiveStatusHandler> logger, 
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<bool>> Handle(SetPluginVersionActiveStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _pluginVersionService.SetActiveStatus(
                request.PluginType,
                request.PluginVersion,
                request.IsActive,
                cancellationToken);

            if (!result)
            {
                return await Result<bool>.FailAsync("Plugin version not found");
            }

            var statusText = request.IsActive ? "enabled" : "disabled";
            return await Result<bool>.SuccessAsync(true, $"Plugin version {statusText} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting plugin version active status");
            return await Result<bool>.FailAsync($"Error: {ex.Message}");
        }
    }
}