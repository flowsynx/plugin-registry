namespace FlowSynx.PluginRegistry.Infrastructure.Models;

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; } = 20;
    public int Skip => (Page - 1) * PageSize;
}
