namespace FlowSynx.Pluginregistry.Models;

public class GitHubReleaseInfo
{
    public string? Version { get; set; }
    public string? LinuxDownloadUrl { get; set; }
    public string? WindowsDownloadUrl { get; set; }
    public string? MacOsDownloadUrl { get; set; }
    public DateTime PublishedAt { get; set; }
    public string? ReleaseNotes { get; set; }
    public bool IsPrerelease { get; set; }
    public long WindowsAssetSize { get; set; }
    public long LinuxAssetSize { get; set; }
    public long MacOsAssetSize { get; set; }
    public int TotalDownloads { get; set; }
}