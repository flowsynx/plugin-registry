using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Command.SetPluginVersionActiveStatus;

public class SetPluginVersionActiveStatusRequest : IRequest<Result<bool>>
{
    public required string PluginType { get; set; } = default!;
    public string PluginVersion { get; set; } = default!;
    public bool IsActive { get; set; }
}