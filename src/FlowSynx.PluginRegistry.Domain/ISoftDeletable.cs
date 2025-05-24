namespace FlowSynx.PluginRegistry.Domain;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}