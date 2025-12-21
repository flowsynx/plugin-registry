using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain;
using FlowSynx.PluginRegistry.Domain.Plugin;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsFullDetailsList;

internal class PluginsFullDetailsListHandler : IRequestHandler<PluginsFullDetailsListRequest, PaginatedResult<PluginsFullDetailsListResponse>>
{
    private readonly ILogger<PluginsFullDetailsListHandler> _logger;
    private readonly IPluginService _pluginService;

    public PluginsFullDetailsListHandler(
        ILogger<PluginsFullDetailsListHandler> logger,
        IPluginService pluginService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginService);
        _logger = logger;
        _pluginService = pluginService;
    }

    public async Task<PaginatedResult<PluginsFullDetailsListResponse>> Handle(PluginsFullDetailsListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            Pagination<PluginEntity> plugins = await _pluginService.All(request.Page ?? 1, cancellationToken);

            var response = plugins.Data.Select(p => new PluginsFullDetailsListResponse
            {
                Type = p.Type,
                Versions = p.Versions
                            .OrderByDescending(x => x.LastModifiedOn)
                            .ThenByDescending(x => x.CreatedOn)
                            .Select(x => x.Version),
                LatestVersion = p.LatestVersion!.Version,
                Owners = p.Owners.Select(x=>x.Profile!.UserName),
                Description = p.LatestVersion!.Description,
                CategoryTitle = p.LatestVersion.PluginCategory.Title,
                LastUpdated = p.LastModifiedOn ?? p.CreatedOn,
                Tags = p.LatestVersion!.PluginVersionTags.Select(x => x.Tag!.Name),
                Specifications = p.LatestVersion!.Specifications.Select(x => new PluginsFullDetailsListSpecification
                {
                    Name = x.Name,
                    Description = x.Description,
                    Type = x.Type,
                    DefaultValue = x.DefaultValue,
                    IsRequired = x.IsRequired
                }).ToList(),
                Operations = p.LatestVersion!.Operations.Select(x => new PluginsFullDetailsListOperation
                {
                    Name = x.Name,
                    Description = x.Description,
                    Parameters = x.Parameters.Select(p => new PluginsFullDetailsListOperationParameter
                    {
                        Name = p.Name,
                        Description = p.Description,
                        Type = p.Type,
                        DefaultValue = p.DefaultValue,
                        IsRequired = p.IsRequired
                    }).ToList()
                }).ToList(),
                TotalDownload = p.Versions.Sum(x=>x.Statistics.Count),
                IsTrusted = p.IsTrusted
            }).ToList();

            return PaginatedResult<PluginsFullDetailsListResponse>.Success(response, plugins.TotalCount, request.Page ?? 1, plugins.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return PaginatedResult<PluginsFullDetailsListResponse>.Failure(ex.ToString());
        }
    }
}