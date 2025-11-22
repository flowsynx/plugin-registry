namespace FlowSynx.Pluginregistry.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TResponse, TData>(string url, TData? data,
        CancellationToken cancellationToken = default);

    Task<TResponse?> PostAsync<TResponse, TData>(string url, TData? data, 
        CancellationToken cancellationToken = default);

    Task<T?> DeleteAsync<T>(string url, CancellationToken cancellationToken = default);

    Task<byte[]> DownloadAsync(string uri);
}