using FlowSynx.PluginRegistry.Domain.ApiKey;
using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public required string Type { get; set; }
    public Guid? LatestVersionId { get; set; }
    public bool IsDeleted { get; set; } = false;

    public PluginVersionEntity? LatestVersion { get; set; }
    public ICollection<PluginVersionEntity> Versions { get; set; } = new List<PluginVersionEntity>();
    public ICollection<ProfilePluginOwnerEntity> Owners { get; set; } = new List<ProfilePluginOwnerEntity>();
    public ICollection<ApiKeyPluginEntity> ApiKeyAssignments { get; set; } = new List<ApiKeyPluginEntity>();
}