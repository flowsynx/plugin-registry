using FlowSynx.Pluginregistry.Models;
using Newtonsoft.Json;
using System.Text.Json;

namespace FlowSynx.Pluginregistry.Services;

public class PluginMetadataReader : IPluginMetadataReader
{
    public async Task<PluginMetadata> ReadAsync(string path)
    {
        var metadataPath = Path.Combine(path, "metadata.json");

        if (!File.Exists(metadataPath))
            throw new FileNotFoundException("metadata.json not found.");

        var content = await File.ReadAllTextAsync(metadataPath);
        var metadata = JsonConvert.DeserializeObject<PluginMetadata>(content);

        if (metadata == null || string.IsNullOrWhiteSpace(metadata.Type) || string.IsNullOrWhiteSpace(metadata.Version))
            throw new InvalidDataException("Invalid or incomplete metadata.json.");

        return metadata;
    }
}