namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginSpecification
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? DefaultValue { get; set; }
    public bool? IsRequired { get; set; } = false;
}