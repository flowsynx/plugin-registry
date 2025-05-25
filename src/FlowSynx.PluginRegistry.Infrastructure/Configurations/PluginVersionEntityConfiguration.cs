using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class PluginVersionEntityConfiguration : IEntityTypeConfiguration<PluginVersionEntity>
{
    public void Configure(EntityTypeBuilder<PluginVersionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
               .IsRequired();

        builder.Property(t => t.PluginId)
               .IsRequired();

        builder.Property(t => t.Description)
               .HasMaxLength(4096);

        builder.Property(t => t.Version)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(v => v.ManifestJson)
               .HasColumnType("jsonb");

        builder.Property(t => t.PluginLocation)
               .HasMaxLength(4096)
               .IsRequired();

        builder.Property(t => t.Url)
               .HasMaxLength(4096);

        builder.HasIndex(v => new { v.PluginId, v.Version })
               .IsUnique();
    }
}