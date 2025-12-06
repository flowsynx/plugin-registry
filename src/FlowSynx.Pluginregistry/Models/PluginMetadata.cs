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

public class SpecificationMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}

public class PluginOperationMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PluginOperationParameterMetadata> Parameters { get; set; } = new List<PluginOperationParameterMetadata>();
}

public class PluginOperationParameterMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}