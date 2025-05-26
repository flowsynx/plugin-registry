using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.ApiKey;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class ApiKeyPluginEntityConfiguration : IEntityTypeConfiguration<ApiKeyPluginEntity>
{
    public void Configure(EntityTypeBuilder<ApiKeyPluginEntity> builder)
    {
        builder.HasKey(x => new { x.ApiKeyId, x.PluginId });

        builder.HasOne(x => x.ApiKey)
            .WithMany(k => k.PluginAssignments)
            .HasForeignKey(x => x.ApiKeyId);

        builder.HasOne(x => x.Plugin)
            .WithMany(k=>k.ApiKeyAssignments)
            .HasForeignKey(x => x.PluginId);
    }
}