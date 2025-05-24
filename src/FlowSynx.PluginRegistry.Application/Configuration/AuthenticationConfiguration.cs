namespace FlowSynx.PluginRegistry.Application.Configuration;

public class AuthenticationConfiguration
{
    public GitHubAuthenticationConfiguration? GitHub { get; set; }
}

public class GitHubAuthenticationConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = string.Empty;
    public int CookieExpireTimeSpanMinutes { get; set; } = 20;
}