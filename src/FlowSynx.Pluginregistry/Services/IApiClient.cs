namespace FlowSynx.Pluginregistry.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url);
    Task<byte[]> DownloadAsync(string uri);
}