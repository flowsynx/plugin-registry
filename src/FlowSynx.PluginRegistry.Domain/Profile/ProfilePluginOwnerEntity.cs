using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Domain.Profile;

public class ProfilePluginOwnerEntity : AuditableEntity
{
    public Guid ProfileId { get; set; }
    public required Guid PluginId { get; set; }
    public ProfileEntity? Profile { get; set; }
    public PluginEntity? Plugin { get; set; }
}