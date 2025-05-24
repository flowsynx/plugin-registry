using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public required string Type { get; set; }
    public required string LatestVersion { get; set; }
    public string? LatestDescription { get; set; }
    public string? LatestTags { get; set; }

    public bool IsDeleted { get; set; } = false;

    public List<PluginVersionEntity> Versions { get; set; } = new();
    public List<ProfilePluginOwnerEntity> Owners { get; set; } = new();
}