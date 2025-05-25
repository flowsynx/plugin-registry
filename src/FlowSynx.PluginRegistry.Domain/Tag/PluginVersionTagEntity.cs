using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Domain.Tag;

public class PluginVersionTagEntity: AuditableEntity
{
    public Guid PluginVersionId { get; set; }
    public Guid TagId { get; set; }
    public PluginVersionEntity? PluginVersion { get; set; }
    public TagEntity? Tag { get; set; }
}