namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;

public class PluginDetailsResponse
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? Url { get; set; }
    public DateTime LastUpdated { get; set; }
    public int DownloadCount { get; set; } = 0;
    public string? Tags { get; set; }
}