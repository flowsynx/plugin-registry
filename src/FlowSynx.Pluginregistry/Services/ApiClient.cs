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
        catch (Exception ex)
        {
            Console.WriteLine("Exception in GetAsync: " + ex);
            return default;
        }
    }

    public async Task<TResponse?> PutAsync<TResponse, TData>(string url, TData? data,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        if (data is null)
        {
            // Send NO CONTENT BODY and NO CONTENT-TYPE
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            response = await _client.SendAsync(request, cancellationToken);
        }
        else
        {
            response = await _client.PutAsJsonAsync(url, data, cancellationToken);
        }

        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength == 0)
            return default;

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    public async Task<byte[]> DownloadAsync(string uri)
    {
        using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }
}       