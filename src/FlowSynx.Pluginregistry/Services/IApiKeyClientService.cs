using FlowSynx.Pluginregistry.Models;
using FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;
using FlowSynx.PluginRegistry.Application.Wrapper;

namespace FlowSynx.Pluginregistry.Services;

public interface IApiKeyClientService
{
    Task<Result<List<ApiKeyViewModel>>?> GetApiKeysAsync(
        CancellationToken cancellationToken = default);

    Task<Result<GenerateApiKeyResponse>?> GenerateApiKeyAsync(
        GenerateApiKeyRequest request, 
        CancellationToken cancellationToken = default);

    Task<Result<bool>?> RevokeApiKeyAsync(
        Guid keyId, 
        CancellationToken cancellationToken = default);
}