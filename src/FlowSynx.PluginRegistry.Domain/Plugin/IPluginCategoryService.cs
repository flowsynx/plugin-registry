namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IPluginCategoryService
{
    Task<IReadOnlyCollection<PluginCategoryEntity>> All(CancellationToken cancellationToken);
    Task<PluginCategoryEntity?> GetByCategoryId(string categoryId, CancellationToken cancellationToken);
}