namespace FlowSynx.PluginRegistry.Domain.Profile;

public class ProfileEntity: AuditableEntity<Guid>, ISoftDeletable
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool IsDeleted { get; set; } = false;

    public List<ProfilePluginOwnerEntity> Owners { get; set; } = new();
}