using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.ApiKey;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class ApiKeyEntityConfiguration : IEntityTypeConfiguration<ApiKeyEntity>
{
    public void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.Key)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(t => t.ProfileId)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(x => x.Profile)
            .WithMany(u => u.ApiKeys)
            .HasForeignKey(x => x.ProfileId);

        builder.HasMany(x => x.PluginAssignments)
            .WithOne(p => p.ApiKey)
            .HasForeignKey(p => p.ApiKeyId);
    }
}