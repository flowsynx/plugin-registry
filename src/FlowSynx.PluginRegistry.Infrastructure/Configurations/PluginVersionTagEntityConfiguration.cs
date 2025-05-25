using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Tag;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class PluginVersionTagEntityConfiguration : IEntityTypeConfiguration<PluginVersionTagEntity>
{
    public void Configure(EntityTypeBuilder<PluginVersionTagEntity> builder)
    {
        builder.HasKey(x => new { x.PluginVersionId, x.TagId });

        builder.HasOne(x => x.PluginVersion)
            .WithMany(pv => pv.PluginVersionTags)
            .HasForeignKey(x => x.PluginVersionId);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.PluginVersionTags)
            .HasForeignKey(x => x.TagId);
    }
}