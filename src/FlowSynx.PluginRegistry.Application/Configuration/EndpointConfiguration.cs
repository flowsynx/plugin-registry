namespace FlowSynx.PluginRegistry.Application.Configuration;

public class EndpointConfiguration
{
    public int? Http { get; set; } = 7236;
    public string? BaseAddress { get; set; } = string.Empty;
    public bool? HttpsRedirection { get; set; } = false;
}