using FlowSynx.Pluginregistry.Models;

namespace FlowSynx.Pluginregistry.Services;

public interface IPluginMetadataReader
{
    Task<PluginMetadata> ReadAsync(string path);
}