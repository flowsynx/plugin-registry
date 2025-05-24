namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IPluginService
{
    Task<IReadOnlyCollection<PluginEntity>> All(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PluginEntity>> AllBySeachQuery(string? query, CancellationToken cancellationToken);
    Task Add(PluginEntity pluginEntity, CancellationToken cancellationToken);
    Task<bool> Delete(PluginEntity pluginEntity, CancellationToken cancellationToken);
}