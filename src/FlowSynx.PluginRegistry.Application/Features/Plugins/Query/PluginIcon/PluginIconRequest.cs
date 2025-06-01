using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginIcon;

public class PluginIconRequest : IRequest<Result<PluginIconResponse>>
{
    public string PluginType { get; set; } = default!;
    public string PluginVersion { get; set; } = default!;
}