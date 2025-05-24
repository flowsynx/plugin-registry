namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

public class PluginsListResponse
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public DateTime LastUpdated { get; set; }
}