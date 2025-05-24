using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class PluginEntityConfiguration : IEntityTypeConfiguration<PluginEntity>
{
    public void Configure(EntityTypeBuilder<PluginEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
               .IsRequired();

        builder.Property(t => t.Type)
               .HasMaxLength(1024)
               .IsRequired();

        builder.Property(t => t.LatestVersion)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(t => t.LatestTags)
               .HasMaxLength(1024);

        builder.Property(t => t.LatestDescription)
               .HasMaxLength(4096);

        builder.HasMany(p => p.Versions)
            .WithOne(o=>o.Plugin)
            .HasForeignKey(v => v.PluginId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Owners)
            .WithOne(o => o.Plugin)
            .HasForeignKey(o => o.PluginId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}