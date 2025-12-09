namespace FlowSynx.Pluginregistry.Models;

public class PluginMetadata
{
    public required Guid Id { get; set; }
    public required string Type { get; set; }
    public required string Version { get; set; }
    public required string CompanyName { get; set; }
    public string? Description { get; set; }
    public string? License { get; set; }
    public string? LicenseUrl { get; set; }
    public string? Icon { get; set; }
    public string? ProjectUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? Copyright { get; set; }
    public string? ReadMe { get; set; }
    public List<string> Authors { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public required string CategoryId { get; set; }
    public required string MinimumFlowSynxVersion { get; set; }
    public string? TargetFlowSynxVersion { get; set; }
    public List<SpecificationMetadata> Specifications { get; set; } = new List<SpecificationMetadata>();
    public List<PluginOperationMetadata> Operations { get; set; } = new List<PluginOperationMetadata>();
}