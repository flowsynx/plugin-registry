using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Domain.ApiKey;

public class ApiKeyEntity: AuditableEntity<Guid>
{
    public required string Key { get; set; }        // Store hashed key
    public string RawKey { get; set; } = null!;     // Optional, used once on generation
    public required Guid ProfileId { get; set; }
    public required string Name { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool CanPushNewPlugins { get; set; } = false;
    public bool CanPushPluginVersions { get; set; } = true;
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    public bool IsActive => !IsRevoked && !IsExpired;

    public ProfileEntity? Profile { get; set; }
    public ICollection<ApiKeyPluginEntity> PluginAssignments { get; set; } = new List<ApiKeyPluginEntity>();
}