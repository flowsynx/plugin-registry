namespace FlowSynx.PluginRegistry.Domain;

public interface IEntity<TId> : IEntity
{
    public TId Id { get; set; }
}

public interface IEntity
{
}