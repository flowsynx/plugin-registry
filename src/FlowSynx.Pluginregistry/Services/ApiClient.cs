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
            Console.WriteLine($"BasedAddress: {_client.BaseAddress}");
            var response = await _client.GetAsync(url);
            Console.WriteLine($"Url: {url}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<T>()
                : default;
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 Exception in GetAsync: " + ex);
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