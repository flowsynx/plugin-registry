using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Domain.Statistic;

public class StatisticEntity: AuditableEntity<Guid>
{
    public Guid PluginId { get; set; }
    public Guid PluginVersionId { get; set; }
    public DateTime DownloadedAt { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }

    public PluginVersionEntity? PluginVersion { get; set; }
}