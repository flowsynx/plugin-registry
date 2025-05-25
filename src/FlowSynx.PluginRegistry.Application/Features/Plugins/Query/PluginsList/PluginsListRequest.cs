using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

public class PluginsListRequest : IRequest<PaginatedResult<PluginsListResponse>>
{
    public string? Query { get; set; }
    public string? Tag { get; set; }
    public int? Page { get; set; }
}