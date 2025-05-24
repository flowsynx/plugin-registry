using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Statistic;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class StatisticEntityConfiguration : IEntityTypeConfiguration<StatisticEntity>
{
    public void Configure(EntityTypeBuilder<StatisticEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
               .IsRequired();

        builder.Property(p => p.IPAddress)
            .HasMaxLength(45);

        builder.Property(p => p.UserAgent)
            .HasMaxLength(1024);

        builder.HasOne(we => we.PluginVersion)
               .WithMany(w => w.Statistics)
               .HasForeignKey(we => we.PluginVersionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.PluginId);
        builder.HasIndex(p => p.PluginVersionId);
    }
}