namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginLocation;

public class PluginLocationResponse
{
    public required string PluginLocation { get; set; }
    public string? Checksum { get; set; }
}