namespace FlowSynx.Pluginregistry.Models;

public class ApiKeyViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool CanPushNewPlugins { get; set; }
    public bool CanPushPluginVersions { get; set; }
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    public bool IsActive => !IsRevoked && !IsExpired;
    public List<string> AssignedPlugins { get; set; } = new();
}