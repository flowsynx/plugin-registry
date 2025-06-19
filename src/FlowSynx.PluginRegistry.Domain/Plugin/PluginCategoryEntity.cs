namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginCategoryEntity : AuditableEntity<Guid>
{
    public required string CategoryId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public ICollection<PluginVersionEntity> PluginVersions { get; set; } = new List<PluginVersionEntity>();
}