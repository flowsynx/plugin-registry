using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class ProfilePluginOwnerEntityConfiguration : IEntityTypeConfiguration<ProfilePluginOwnerEntity>
{
    public void Configure(EntityTypeBuilder<ProfilePluginOwnerEntity> builder)
    {
        builder.HasKey(uo => new { uo.ProfileId, uo.PluginId });

        builder.HasOne(uo => uo.Profile)
            .WithMany(u => u.Owners)
            .HasForeignKey(uo => uo.ProfileId);

        builder.HasOne(uo => uo.Plugin)
            .WithMany(p => p.Owners)
            .HasForeignKey(uo => uo.PluginId);
    }
}