using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginReadme;

internal class PluginReadmeHandler : IRequestHandler<PluginReadmeRequest, Result<PluginReadmeResponse>>
{
    private readonly ILogger<PluginReadmeHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public PluginReadmeHandler(
        ILogger<PluginReadmeHandler> logger,
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<PluginReadmeResponse>> Handle(PluginReadmeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginVersionService.GetByPluginType(request.PluginType, request.PluginVersion, cancellationToken);
            if (plugin is null)
                throw new Exception($"Plugin details for '{request.PluginType}' v{request.PluginVersion} could not be found");
            
            var response = new PluginReadmeResponse
            {
                Readme = plugin.ReadMe,
                Description = plugin.Description,
            };

            return await Result<PluginReadmeResponse>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<PluginReadmeResponse>.FailAsync(ex.ToString());
        }
    }
}