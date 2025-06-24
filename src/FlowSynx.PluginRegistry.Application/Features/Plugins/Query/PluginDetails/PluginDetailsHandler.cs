﻿using FlowSynx.PluginRegistry.Application.Wrapper;
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
                ProjectUrl = plugin.ProjectUrl,
                RepositoryUrl = plugin.RepositoryUrl,
                Copyright = plugin.Copyright,
                CategoryTitle = plugin.PluginCategory.Title,
                License = plugin.License,
                LicenseUrl = plugin.LicenseUrl,
                Icon = plugin.Icon,
                LastUpdated = plugin.LastModifiedOn ?? plugin.CreatedOn,
                TotalDownload = plugin.Statistics.Count,
                Checksum = plugin.Checksum,
                Tags = plugin.PluginVersionTags.Select(x=>x.Tag!.Name),
                Versions = plugin.Plugin.Versions
                                 .OrderByDescending(x=>x.LastModifiedOn)
                                 .ThenByDescending(x=>x.CreatedOn)
                                 .Select(x=>x.Version)
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