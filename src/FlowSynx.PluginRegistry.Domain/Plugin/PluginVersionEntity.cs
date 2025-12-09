using FlowSynx.PluginRegistry.Domain.Statistic;
using FlowSynx.PluginRegistry.Domain.Tag;

namespace FlowSynx.PluginRegistry.Domain.Plugin;

public class PluginVersionEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public required Guid PluginId { get; set; }
    public required string Version { get; set; }
    public string? Description { get; set; }
    public required string PluginLocation { get; set; }
    public List<string> Authors { get; set; } = new List<string>();
    public string? License { get; set; }
    public string? LicenseUrl { get; set; }
    public string? Icon { get; set; }
    public string? ProjectUrl { get; set; }
    public string? Copyright { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? ReadMe { get; set; }
    public required Guid PluginCategoryId { get; set; }
    public required string MinimumFlowSynxVersion { get; set; }
    public string? TargetFlowSynxVersion { get; set; }
    public List<PluginSpecification> Specifications { get; set; } = new List<PluginSpecification>();
    public List<PluginOperation> Operations { get; set; } = new List<PluginOperation>();
    public bool? IsLatest { get; set; }
    public string? MetadataFile { get; set; }
    public string? Checksum { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public PluginEntity Plugin { get; set; } = default!;
    public PluginCategoryEntity PluginCategory { get; set; } = default!;
    public ICollection<StatisticEntity> Statistics { get; set; } = new List<StatisticEntity>();
    public ICollection<PluginVersionTagEntity> PluginVersionTags { get; set; } = new List<PluginVersionTagEntity>();
}