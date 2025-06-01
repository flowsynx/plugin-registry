using FlowSynx.Pluginregistry.Models;
using System.Text.Json;

namespace FlowSynx.Pluginregistry.Services;

public class PluginMetadataReader : IPluginMetadataReader
{
    public async Task<PluginMetadata> ReadAsync(string path)
    {
        var manifestPath = Path.Combine(path, "manifest.json");

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException("manifest.json not found.");

        var content = await File.ReadAllTextAsync(manifestPath);
        var metadata = JsonSerializer.Deserialize<PluginMetadata>(content);

        if (metadata == null || string.IsNullOrWhiteSpace(metadata.Type) || string.IsNullOrWhiteSpace(metadata.Version))
            throw new InvalidDataException("Invalid or incomplete manifest.json.");

        return metadata;
    }
}