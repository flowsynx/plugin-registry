namespace FlowSynx.PluginRegistry.Domain.Tag;

public class TagEntity: AuditableEntity<Guid>
{
    public required string Name { get; set; }

    public ICollection<PluginVersionTagEntity> PluginVersionTags { get; set; } = new List<PluginVersionTagEntity>();
}