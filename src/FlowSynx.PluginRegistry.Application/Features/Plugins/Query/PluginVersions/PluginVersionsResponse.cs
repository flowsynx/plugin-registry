namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;

public class PluginVersionsResponse
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public bool? IsLatest { get; set; }
    public int? Downloads { get; set; } = 0;
    public DateTime LastUpdated { get; set; }
}