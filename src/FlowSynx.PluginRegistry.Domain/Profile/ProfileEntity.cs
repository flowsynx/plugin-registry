using FlowSynx.PluginRegistry.Domain.ApiKey;

namespace FlowSynx.PluginRegistry.Domain.Profile;

public class ProfileEntity: AuditableEntity<Guid>, ISoftDeletable
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ICollection<ProfilePluginOwnerEntity> Owners { get; set; } = new List<ProfilePluginOwnerEntity>();
    public ICollection<ApiKeyEntity> ApiKeys { get; set; } = new List<ApiKeyEntity>();
}