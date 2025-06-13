using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;

public class PluginVersionsRequest : IRequest<Result<IEnumerable<PluginVersionsResponse>>>
{
    public string PluginType { get; set; } = default!;
}