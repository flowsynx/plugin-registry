using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Domain.ApiKey;
using FlowSynx.PluginRegistry.Infrastructure.Helpers;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<ProfileService> _logger;

    public ApiKeyService(
        IDbContextFactory<ApplicationContext> appContextFactory, 
        ILogger<ProfileService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<ApiKeyEntity>?> GetByUserId(Guid profileId, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var result = await context.ApiKeys
                .Where(p=>p.ProfileId == profileId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(ApiKeyEntity apiKeyEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            await context.ApiKeys
                .AddAsync(apiKeyEntity, cancellationToken)
                .ConfigureAwait(false);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ApiKeyEntity> GenerateKey(
        string name, 
        Guid profileId,
        bool? canPushNewPlugins,
        bool? canPushPluginVersions,
        DateTime? expiresAt, 
        List<Guid>? pluginIds, 
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var rawKey = ApiKeyHelper.Generate();
            var hashed = ApiKeyHelper.Hash(rawKey);

            var apiKeyId = Guid.NewGuid();
            var apiKey = new ApiKeyEntity
            {
                Id = apiKeyId,
                Name = name,
                Key = hashed,
                RawKey = rawKey,
                ProfileId = profileId,
                ExpiresAt = expiresAt,
                CanPushNewPlugins = canPushNewPlugins ?? false,
                CanPushPluginVersions = canPushPluginVersions ?? false,
                PluginAssignments = pluginIds?.Select(pid => new ApiKeyPluginEntity
                {
                    ApiKeyId = apiKeyId,
                    PluginId = pid 
                }).ToList() ?? new()
            };

            await context.ApiKeys
                .AddAsync(apiKey, cancellationToken)
                .ConfigureAwait(false);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return apiKey;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> ValidateApiKeyAsync(string rawKey, Guid pluginId, CancellationToken cancellationToken)
    {
        using var context = _appContextFactory.CreateDbContext();
        var hash = ApiKeyHelper.Hash(rawKey);

        var apiKey = await context.ApiKeys
            .Include(k => k.PluginAssignments)
            .FirstOrDefaultAsync(k =>
                k.Key == hash &&
                !k.IsRevoked &&
                (k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow));

        if (apiKey == null) 
            return false;

        if (apiKey.PluginAssignments.Any())
            return apiKey.PluginAssignments.Any(p => p.PluginId == pluginId);

        return true;
    }

    public async Task<bool> RevokeKeyAsync(Guid keyId, Guid profileId, CancellationToken cancellationToken)
    {
        using var context = _appContextFactory.CreateDbContext();
        var key = await context.ApiKeys.FirstOrDefaultAsync(k => k.Id == keyId && k.ProfileId == profileId);
        if (key == null || key.IsRevoked)
            return false;

        key.IsRevoked = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RevokeKeyByRawAsync(string rawKey, Guid profileId, CancellationToken cancellationToken)
    {
        using var context = _appContextFactory.CreateDbContext();
        var hashed = ApiKeyHelper.Hash(rawKey);
        var key = await context.ApiKeys.FirstOrDefaultAsync(k => k.Key == hashed && k.ProfileId == profileId);

        if (key == null || key.IsRevoked)
            return false;

        key.IsRevoked = true;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CanPushAsync(string apiKey, Guid? pluginId, Guid currentUserId, bool isNewPackage,
        CancellationToken cancellationToken)
    {
        using var context = _appContextFactory.CreateDbContext();
        var hashedKey = ApiKeyHelper.Hash(apiKey);

        var key = await context.ApiKeys
            .Include(k => k.PluginAssignments)
            .FirstOrDefaultAsync(k => k.Key == hashedKey && !k.IsRevoked && k.ProfileId == currentUserId);

        if (key == null || key.IsExpired)
            return false;

        // If plugin assignments exist, validate against them
        if (key.PluginAssignments.Any())
        {
            if (!isNewPackage && pluginId.HasValue && !key.PluginAssignments.Any(p => p.PluginId == pluginId.Value))
                return false;
        }

        // Check permissions based on operation type
        if (isNewPackage && !key.CanPushNewPlugins)
            return false;

        if (!isNewPackage && !key.CanPushPluginVersions)
            return false;

        return true;
    }
}