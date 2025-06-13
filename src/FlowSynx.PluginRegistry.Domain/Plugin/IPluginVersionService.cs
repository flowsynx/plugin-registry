namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IPluginVersionService
{
    Task<IReadOnlyCollection<PluginVersionEntity>> AllByPliginId(Guid pluginId, CancellationToken cancellationToken);
    Task<PluginVersionEntity?> GetByPluginType(string pluginType, string pluginVersion, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PluginVersionEntity>> GetVersionsByPluginType(string pluginType, CancellationToken cancellationToken);
    Task Add(PluginVersionEntity pluginVersionEntity, CancellationToken cancellationToken);
    Task AddTagsToPluginVersionAsync(Guid pluginVersionId, List<string> tagNames, CancellationToken cancellationToken);
    Task Update(PluginVersionEntity pluginVersionEntity, CancellationToken cancellationToken);
    Task<bool> Delete(PluginVersionEntity pluginVersionEntity, CancellationToken cancellationToken);
}