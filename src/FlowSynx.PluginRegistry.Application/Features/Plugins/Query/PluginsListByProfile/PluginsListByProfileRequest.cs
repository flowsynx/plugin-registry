using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;

public class PluginsListByProfileRequest : IRequest<PaginatedResult<PluginsListByProfileResponse>>
{
    public string UserName { get; set; } = default!;
    public int? Page { get; set; }
}