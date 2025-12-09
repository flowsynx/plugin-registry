namespace FlowSynx.Pluginregistry.Models;

public class PluginOperationMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<PluginOperationParameterMetadata> Parameters { get; set; } = new List<PluginOperationParameterMetadata>();
}