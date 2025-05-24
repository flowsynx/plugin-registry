using FlowSynx.PluginRegistry.Application.Services;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class SystemClock : ISystemClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}