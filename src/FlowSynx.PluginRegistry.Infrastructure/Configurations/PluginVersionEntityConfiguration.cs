using FlowSynx.PluginRegistry.Domain.Plugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            v => System.Text.Json.JsonSerializer.Serialize(v),
            v => System.Text.Json.JsonSerializer.Deserialize<List<PluginSpecification>?>(v)
        );

        var pluginSpecificationComparer = new ValueComparer<List<PluginSpecification>>(
            (c1, c2) => System.Text.Json.JsonSerializer.Serialize(c1) ==
                        System.Text.Json.JsonSerializer.Serialize(c2),
            c => System.Text.Json.JsonSerializer.Serialize(c).GetHashCode(),
            c => System.Text.Json.JsonSerializer.Deserialize<List<PluginSpecification>>(System.Text.Json.JsonSerializer.Serialize(c))
        );

        builder.Property(e => e.Specifications)
               .HasColumnType("jsonb")
               .HasConversion(pluginSpecificationConverter, pluginSpecificationComparer);

        // JSON serialization for plugin operations
        var pluginOperationConverter = new ValueConverter<List<PluginOperation>?, string>(
            v => System.Text.Json.JsonSerializer.Serialize(v),
            v => System.Text.Json.JsonSerializer.Deserialize<List<PluginOperation>?>(v)
        );

        var pluginOperationComparer = new ValueComparer<List<PluginOperation>>(
            (c1, c2) => System.Text.Json.JsonSerializer.Serialize(c1) ==
                        System.Text.Json.JsonSerializer.Serialize(c2),
            c => System.Text.Json.JsonSerializer.Serialize(c).GetHashCode(),
            c => System.Text.Json.JsonSerializer.Deserialize<List<PluginOperation>>(System.Text.Json.JsonSerializer.Serialize(c))
        );

        builder.Property(e => e.Operations)
               .HasColumnType("jsonb")
               .HasConversion(pluginOperationConverter, pluginOperationComparer);

        builder.HasIndex(v => new { v.PluginId, v.Version })
               .IsUnique();
    }
}