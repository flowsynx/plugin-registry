using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;

public class PluginsStatisticsByProfileRequest : IRequest<Result<PluginsStatisticsByProfileResponse>>
{
    public string UserName { get; set; } = default!;
}