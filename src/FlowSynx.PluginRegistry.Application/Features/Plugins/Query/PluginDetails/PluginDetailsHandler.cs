using FlowSynx.PluginRegistry.Application.Exceptions;
using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;

internal class PluginDetailsHandler : IRequestHandler<PluginDetailsRequest, Result<PluginDetailsResponse>>
{
    private readonly ILogger<PluginDetailsHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;

    public PluginDetailsHandler(
        ILogger<PluginDetailsHandler> logger,
        IPluginVersionService pluginVersionService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
    }

    public async Task<Result<PluginDetailsResponse>> Handle(PluginDetailsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _pluginVersionService.GetByPluginType(request.PluginType, request.PluginVersion, cancellationToken);
            if (plugin is null)
                throw new Exception($"Plugin details for '{request.PluginType}' v{request.PluginVersion} could not be found");
            
            var response = new PluginDetailsResponse
            {
                Type = plugin.Plugin.Type,
                Version = plugin.Version,
                Owners = plugin.Plugin.Owners.Select(x=>x.Profile!.UserName),
                Description = plugin.Description,
                Url = plugin.Url,
                LastUpdated = plugin.LastModifiedOn ?? plugin.CreatedOn,
                DownloadCount = plugin.Statistics.Count(x=>x.PluginVersionId == plugin.Id),
                Tags = plugin.PluginVersionTags.Select(x=>x.Tag!.Name),
                Versions = plugin.Plugin.Versions.Select(x=>x.Version)
            };

            return await Result<PluginDetailsResponse>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<PluginDetailsResponse>.FailAsync(ex.ToString());
        }
    }
}