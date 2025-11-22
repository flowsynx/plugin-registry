namespace FlowSynx.Pluginregistry.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url);
    Task<TResponse?> PutAsync<TResponse, TData>(string url, TData? data,
        CancellationToken cancellationToken = default);
    Task<byte[]> DownloadAsync(string uri);
}