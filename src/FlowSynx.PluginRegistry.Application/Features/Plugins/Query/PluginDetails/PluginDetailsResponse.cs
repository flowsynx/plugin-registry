namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;

public class PluginDetailsResponse
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public string? License { get; set; }
    public string? LicenseUrl { get; set; }
    public string? Icon { get; set; }
    public string? ProjectUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? Copyright { get; set; }
    public string? CategoryTitle { get; set; }
    public required string MinimumFlowSynxVersion { get; set; }
    public string? TargetFlowSynxVersion { get; set; }
    public DateTime LastUpdated { get; set; }
    public int TotalDownload { get; set; } = 0;
    public bool IsTrusted { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> Versions { get; set; } = new List<string>();
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? Checksum { get; set; }
}