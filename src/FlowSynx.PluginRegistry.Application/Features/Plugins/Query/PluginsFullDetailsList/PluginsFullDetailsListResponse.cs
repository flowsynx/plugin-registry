namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsFullDetailsList;

public class PluginsFullDetailsListResponse
{
    public required string Type { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Versions { get; set; } = new List<string>();
    public string LatestVersion { get; set; } = string.Empty;
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? CategoryTitle { get; set; }
    public List<PluginsFullDetailsListSpecification> Specifications { get; set; } = new List<PluginsFullDetailsListSpecification>();
    public List<PluginsFullDetailsListOperation> Operations { get; set; } = new List<PluginsFullDetailsListOperation>();
    public DateTime LastUpdated { get; set; }
    public int TotalDownload { get; set; } = 0;
    public bool IsTrusted { get; set; } = false;
}

public class PluginsFullDetailsListOperation
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PluginsFullDetailsListOperationParameter> Parameters { get; set; } = new List<PluginsFullDetailsListOperationParameter>();
}

public class PluginsFullDetailsListOperationParameter
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}

public class PluginsFullDetailsListSpecification
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}