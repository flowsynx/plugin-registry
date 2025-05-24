using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Domain.Profile;

namespace FlowSynx.PluginRegistry.Infrastructure.Configurations;

public class ProfileEntityConfiguration : IEntityTypeConfiguration<ProfileEntity>
{
    public void Configure(EntityTypeBuilder<ProfileEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Id)
               .IsRequired();

        builder.Property(t => t.UserId)
               .IsRequired();

        builder.Property(t => t.UserName)
               .HasMaxLength(1024)
               .IsRequired();

        builder.HasIndex(u => u.UserName)
               .IsUnique();

        builder.Property(t => t.Email)
               .HasMaxLength(1024)
               .IsRequired();
    }
}