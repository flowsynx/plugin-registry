namespace FlowSynx.Pluginregistry.Models;

public class GitHubReleaseInfo
{
    public string? Version { get; set; }
    public string? LinuxDownloadUrl { get; set; }
    public string? WindowsDownloadUrl { get; set; }
    public string? MacOsDownloadUrl { get; set; }
}