using FlowSynx.PluginRegistry.Domain.Statistic;
using FlowSynx.PluginRegistry.Domain.Tag;

namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginVersionEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public required Guid PluginId { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public string? ManifestJson { get; set; }
    public required string PluginLocation { get; set; }
    public string? Url { get; set; }
    public bool? IsLatest { get; set; }
    public bool IsDeleted { get; set; } = false;

    public PluginEntity Plugin { get; set; } = default!;
    public ICollection<StatisticEntity> Statistics { get; set; } = new List<StatisticEntity>();
    public ICollection<PluginVersionTagEntity> PluginVersionTags { get; set; } = new List<PluginVersionTagEntity>();
}