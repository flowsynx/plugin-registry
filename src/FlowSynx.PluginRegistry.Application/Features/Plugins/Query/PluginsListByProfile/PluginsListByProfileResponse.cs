namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;

public class PluginsListByProfileResponse
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? CategoryTitle { get; set; }
    public DateTime LastUpdated { get; set; }
    public long TotalDownload { get; set; } = 0;
    public bool IsTrusted { get; set; } = false;
}