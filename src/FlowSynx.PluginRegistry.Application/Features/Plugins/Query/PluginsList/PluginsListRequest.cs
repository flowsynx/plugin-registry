using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

public class PluginsListRequest : IRequest<Result<IEnumerable<PluginsListResponse>>>
{
    public string? Query { get; set; }
}