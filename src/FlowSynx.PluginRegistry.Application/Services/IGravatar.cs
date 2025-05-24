namespace FlowSynx.PluginRegistry.Application.Services;

public interface IGravatar
{
    string GetGravatarUrl(string email, int size = 100);
}