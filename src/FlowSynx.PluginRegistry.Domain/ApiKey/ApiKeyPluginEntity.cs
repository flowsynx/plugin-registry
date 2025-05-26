using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Domain.ApiKey;

public class ApiKeyPluginEntity
{
    public Guid ApiKeyId { get; set; }
    public ApiKeyEntity ApiKey { get; set; } = null!;

    public Guid PluginId { get; set; }
    public PluginEntity Plugin { get; set; } = null!;
}