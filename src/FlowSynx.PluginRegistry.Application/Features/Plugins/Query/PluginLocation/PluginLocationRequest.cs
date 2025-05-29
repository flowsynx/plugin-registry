using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginLocation;

public class PluginLocationRequest : IRequest<Result<PluginLocationResponse>>
{
    public string PluginType { get; set; } = default!;
    public string PluginVersion { get; set; } = default!;
}