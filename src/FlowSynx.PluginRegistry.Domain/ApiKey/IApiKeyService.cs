namespace FlowSynx.PluginRegistry.Domain.ApiKey;

public interface IApiKeyService
{
    Task<IReadOnlyCollection<ApiKeyEntity>?> GetByUserId(Guid profileId, CancellationToken cancellationToken);

    Task Add(ApiKeyEntity apiKeyEntity, CancellationToken cancellationToken);

    Task<(string rawKey, ApiKeyEntity savedKey)> GenerateKey(string name, Guid profileId, DateTime? expiresAt, 
        List<Guid>? pluginIds, CancellationToken cancellationToken);

    Task<bool> ValidateApiKeyAsync(string rawKey, Guid pluginId, CancellationToken cancellationToken);

    Task<bool> RevokeKeyAsync(Guid keyId, Guid profileId, CancellationToken cancellationToken);

    Task<bool> RevokeKeyByRawAsync(string rawKey, Guid profileId, CancellationToken cancellationToken);

    Task<bool> CanPushAsync(string apiKey, Guid? pluginId, Guid currentUserId, bool isNewPackage, 
        CancellationToken cancellationToken);
}