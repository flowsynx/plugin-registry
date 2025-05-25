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

        builder.HasMany(p => p.Versions)
               .WithOne(o => o.Plugin)
               .HasForeignKey(v => v.PluginId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Owners)
               .WithOne(o => o.Plugin)
               .HasForeignKey(o => o.PluginId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.LatestVersion)
               .WithMany()
               .HasForeignKey(p => p.LatestVersionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}