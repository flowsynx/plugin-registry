using FlowSynx.Pluginregistry.Models;

namespace FlowSynx.Pluginregistry.Services;

public interface IGitHubReleaseService
{
    Task<List<GitHubReleaseInfo>> GetAllReleasesAsync();
}