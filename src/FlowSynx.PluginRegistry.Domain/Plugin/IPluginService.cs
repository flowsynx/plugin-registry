namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IPluginService
{
    Task<Pagination<PluginEntity>> All(int page, CancellationToken cancellationToken);
    Task<PluginEntity?> GetByPluginType(string pluginType, CancellationToken cancellationToken);
    Task<Pagination<PluginEntity>> AllBySeachQuery(string? query, int page, CancellationToken cancellationToken);
    Task<Pagination<PluginEntity>> AllBySeachTags(string? tag, int page, CancellationToken cancellationToken);
    Task<Pagination<PluginEntity>> AllByProfileUserName(string username, int page, CancellationToken cancellationToken);
    Task Add(PluginEntity pluginEntity, CancellationToken cancellationToken);
    Task Update(PluginEntity pluginEntity, CancellationToken cancellationToken);
    Task<bool> Delete(PluginEntity pluginEntity, CancellationToken cancellationToken);
}