﻿using FlowSynx.PluginRegistry.Domain.Statistic;
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
    public bool? IsLatest { get; set; }
    public string? ManifestJson { get; set; }
    public string? Checksum { get; set; }
    public bool IsDeleted { get; set; } = false;

    public PluginEntity Plugin { get; set; } = default!;
    public ICollection<StatisticEntity> Statistics { get; set; } = new List<StatisticEntity>();
    public ICollection<PluginVersionTagEntity> PluginVersionTags { get; set; } = new List<PluginVersionTagEntity>();
}