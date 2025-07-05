using FlowSynx.Pluginregistry.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FlowSynx.Pluginregistry.Services;

public class GitHubReleaseService : IGitHubReleaseService
{
    private readonly HttpClient _httpClient;

    public GitHubReleaseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GitHubReleaseInfo>> GetAllReleasesAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.github.com/repos/flowsynx/flowpack/releases");

        // GitHub API requires User-Agent header
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("BlazorApp", "1.0"));

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new List<GitHubReleaseInfo>();

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var releases = new List<GitHubReleaseInfo>();

        foreach (var release in root.EnumerateArray())
        {
            var releaseInfo = new GitHubReleaseInfo();

            if (release.TryGetProperty("tag_name", out var tagName))
            {
                releaseInfo.Version = tagName.GetString();
            }

            if (release.TryGetProperty("assets", out var assets) && assets.GetArrayLength() > 0)
            {
                foreach (var asset in assets.EnumerateArray())
                {
                    if (asset.TryGetProperty("browser_download_url", out var downloadUrl))
                    {
                        var url = downloadUrl.GetString();

                        if (string.IsNullOrEmpty(url))
                            continue;

                        if (url.Contains("windows-x64", StringComparison.OrdinalIgnoreCase) &&
                        url.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            releaseInfo.WindowsDownloadUrl = url;
                        }
                        else if (url.Contains("linux-x64", StringComparison.OrdinalIgnoreCase) &&
                            url.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
                        {
                            releaseInfo.LinuxDownloadUrl = url;
                        }
                        else if (url.Contains("osx-x64", StringComparison.OrdinalIgnoreCase) &&
                            url.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
                        {
                            releaseInfo.MacOsDownloadUrl = url;
                        }
                    }
                }
            }

            releases.Add(releaseInfo);
        }

        return releases;
    }
}