using FlowSynx.PluginRegistry.Domain.Plugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

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

        builder.Property(v => v.MetadataFile)
               .HasMaxLength(4096);

        builder.Property(t => t.PluginLocation)
               .HasMaxLength(4096)
               .IsRequired();

        builder.Property(t => t.RepositoryUrl)
               .HasMaxLength(4096);

        builder.Property(t => t.ProjectUrl)
               .HasMaxLength(4096);

        builder.Property(t => t.Copyright)
               .HasMaxLength(2048);

        builder.Property(t => t.Icon)
               .HasMaxLength(4096);

        builder.Property(t => t.License)
               .HasMaxLength(1024);

        builder.Property(t => t.License)
               .HasMaxLength(1024);

        builder.Property(t => t.LicenseUrl)
               .HasMaxLength(4096);

        builder.Property(t => t.MinimumFlowSynxVersion)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(t => t.TargetFlowSynxVersion)
               .HasMaxLength(50);

        // JSON serialization for plugin specifications
        var pluginSpecificationConverter = new ValueConverter<List<PluginSpecification>?, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<PluginSpecification>?>(v)
        );

        builder.Property(e => e.Specifications)
               .HasColumnType("jsonb")
               .HasConversion(pluginSpecificationConverter);

        // JSON serialization for plugin operations
        var pluginOperationConverter = new ValueConverter<List<PluginOperation>?, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<PluginOperation>?>(v)
        );

        builder.Property(e => e.Operations)
               .HasColumnType("jsonb")
               .HasConversion(pluginOperationConverter);

        builder.HasIndex(v => new { v.PluginId, v.Version })
               .IsUnique();
    }
}