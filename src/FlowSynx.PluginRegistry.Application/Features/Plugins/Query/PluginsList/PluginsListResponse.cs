namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsList;

public class PluginsListResponse
{
    public required string Type { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> Owners { get; set; } = new List<string>();
    public string? CategoryTitle { get; set; }
    public List<PluginsListSpecification> Specifications { get; set; } = new List<PluginsListSpecification>();
    public List<PluginsListOperation> Operations { get; set; } = new List<PluginsListOperation>();
    public DateTime LastUpdated { get; set; }
    public int TotalDownload { get; set; } = 0;
    public bool IsTrusted { get; set; } = false;
}

public class PluginsListOperation
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PluginsListOperationParameter> Parameters { get; set; } = new List<PluginsListOperationParameter>();
}

public class PluginsListOperationParameter
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}

public class PluginsListSpecification
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}