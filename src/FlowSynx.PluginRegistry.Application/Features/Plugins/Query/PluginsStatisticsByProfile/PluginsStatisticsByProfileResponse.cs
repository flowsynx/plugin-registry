namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;

public class PluginsStatisticsByProfileResponse
{
    public string? Email { get; set; }
    public int TotalCount { get; set; } = 0;
    public long TotalDownload { get; set; } = 0;
}