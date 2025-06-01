using System.Net.Http;

namespace FlowSynx.Pluginregistry.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _client;

    public ApiClient(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("Api");
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _client.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>()
                : default;
        }
        catch
        {
            return default;
        }
    }

    public async Task<byte[]> DownloadAsync(string uri)
    {
        using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}