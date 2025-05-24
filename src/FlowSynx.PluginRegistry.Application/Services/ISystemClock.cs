namespace FlowSynx.PluginRegistry.Application.Services;

public interface ISystemClock
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}