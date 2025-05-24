namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IPluginVersionService
{
    Task<IReadOnlyCollection<PluginVersionEntity>> AllByPliginId(Guid pluginId, CancellationToken cancellationToken);
    Task<PluginVersionEntity?> GetByPluginId(Guid pluginVersionId, CancellationToken cancellationToken);
    Task<PluginVersionEntity?> GetByPluginType(string pluginType, string pluginVersion, CancellationToken cancellationToken);
    Task<bool> Delete(PluginVersionEntity pluginEntity, CancellationToken cancellationToken);
}