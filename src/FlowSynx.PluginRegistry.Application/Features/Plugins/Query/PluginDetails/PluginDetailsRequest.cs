using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;

public class PluginDetailsRequest : IRequest<Result<PluginDetailsResponse>>
{
    public string? PluginType { get; set; }
    public string? PluginVersion { get; set; }
}