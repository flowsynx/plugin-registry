using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class PluginCategoryEntityConfiguration : IEntityTypeConfiguration<PluginCategoryEntity>
{
    public void Configure(EntityTypeBuilder<PluginCategoryEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
               .IsRequired();

        builder.Property(t => t.CategoryId)
               .HasMaxLength(256)
               .IsRequired();

        builder.Property(t => t.Description)
               .HasMaxLength(4096);

        builder.HasMany(p => p.PluginVersions)
               .WithOne(o => o.PluginCategory)
               .HasForeignKey(v => v.PluginCategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}