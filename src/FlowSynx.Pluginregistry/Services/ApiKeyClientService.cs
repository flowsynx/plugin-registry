using FlowSynx.Pluginregistry.Models;
using FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;
using FlowSynx.PluginRegistry.Application.Wrapper;

namespace FlowSynx.Pluginregistry.Services;

public class ApiKeyClientService : IApiKeyClientService
{
    private readonly IApiClient _apiClient;

    public ApiKeyClientService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<Result<List<ApiKeyViewModel>>?> GetApiKeysAsync(CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAsync<Result<List<ApiKeyViewModel>>>("/api/apikeys", cancellationToken);
    }

    public async Task<Result<GenerateApiKeyResponse>?> GenerateApiKeyAsync(GenerateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _apiClient.PostAsync<Result<GenerateApiKeyResponse>, GenerateApiKeyRequest>("/api/apikeys", request, cancellationToken);

        if (result?.Succeeded == true && result.Data != null)
        {
            return await Result<GenerateApiKeyResponse>.SuccessAsync(result.Data, string.Join(", ", result.Messages));
        }

        return await Result<GenerateApiKeyResponse>.FailAsync(string.Join(", ", result.Messages) ?? "Failed to create API key");
    }

    public Task<Result<bool>?> RevokeApiKeyAsync(Guid keyId, CancellationToken cancellationToken = default)
    {
        return _apiClient.DeleteAsync<Result<bool>>($"/api/apikeys/{keyId}", cancellationToken);
    }
}