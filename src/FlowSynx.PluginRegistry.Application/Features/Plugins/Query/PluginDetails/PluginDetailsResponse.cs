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
    public List<PluginDetailsOSpecification> Specifications { get; set; } = new List<PluginDetailsOSpecification>();
    public List<PluginDetailsOperation> Operations { get; set; } = new List<PluginDetailsOperation>();
    public DateTime LastUpdated { get; set; }
    public int TotalDownload { get; set; } = 0;
    public bool IsTrusted { get; set; } = false;
    public bool IsActive { get; set; } = false;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> Versions { get; set; } = new List<string>();
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? Checksum { get; set; }
}

public class PluginDetailsOperation
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PluginDetailsOperationParameter> Parameters { get; set; } = new List<PluginDetailsOperationParameter>();
}

public class PluginDetailsOperationParameter
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}

public class PluginDetailsOSpecification
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}