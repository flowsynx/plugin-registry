using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginReadme;

public class PluginReadmeRequest : IRequest<Result<PluginReadmeResponse>>
{
    public string PluginType { get; set; } = default!;
    public string PluginVersion { get; set; } = default!;
}