using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsFullDetailsList;

public class PluginsFullDetailsListRequest : IRequest<PaginatedResult<PluginsFullDetailsListResponse>>
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}