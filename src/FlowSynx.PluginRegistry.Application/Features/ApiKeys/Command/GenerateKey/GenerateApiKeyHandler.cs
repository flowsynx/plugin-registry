using FlowSynx.PluginRegistry.Application.Services;
using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.ApiKey;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Profile;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;

internal class GenerateApiKeyHandler : IRequestHandler<GenerateApiKeyRequest, Result<GenerateApiKeyResponse>>
{
    private readonly ILogger<GenerateApiKeyHandler> _logger;
    private readonly IPluginVersionService _pluginVersionService;
    private readonly IProfileService _profileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApiKeyService _apiKeyService;

    public GenerateApiKeyHandler(
        ILogger<GenerateApiKeyHandler> logger, 
        IPluginVersionService pluginVersionService,
        IProfileService profileService,
        ICurrentUserService currentUserService,
        IApiKeyService apiKeyService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(pluginVersionService);
        ArgumentNullException.ThrowIfNull(profileService);
        ArgumentNullException.ThrowIfNull(currentUserService);
        ArgumentNullException.ThrowIfNull(apiKeyService);
        _logger = logger;
        _pluginVersionService = pluginVersionService;
        _profileService = profileService;
        _currentUserService = currentUserService;
        _apiKeyService = apiKeyService;
    }

    public async Task<Result<GenerateApiKeyResponse>> Handle(GenerateApiKeyRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _currentUserService.ValidateAuthentication();

            var profileId = await GetCurrentUserProfileId(cancellationToken);
            if (profileId == Guid.Empty)
                return await Result<GenerateApiKeyResponse>.FailAsync("User profile not found");

            var result = await _apiKeyService.GenerateKey(
                request.Name,
                profileId,
                request.CanPushNewPlugins,
                request.CanPushPluginVersions,
                request.ExpiresAt,
                request.PluginIds,
                cancellationToken);

            var response = new GenerateApiKeyResponse
            {
                Id = result.Id,
                Name = result.Name,
                RawKey = result.RawKey,
                Key = result.Key,
                CreatedAt = result.CreatedOn,
                ExpiresAt = result.ExpiresAt,
                IsRevoked = result.IsRevoked,
                CanPushNewPlugins = result.CanPushNewPlugins,
                CanPushPluginVersions = result.CanPushPluginVersions,
                AssignedPlugins = result.PluginAssignments?.Select(p => p.PluginId.ToString()).ToList() ?? new()
            };

            return await Result<GenerateApiKeyResponse>.SuccessAsync(response, "API key generating successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating API key");
            return await Result<GenerateApiKeyResponse>.FailAsync($"Error generating API key: {ex.Message}");
        }
    }

    private async Task<Guid> GetCurrentUserProfileId(CancellationToken cancellationToken)
    {
        var profile = await _profileService.GetByUserId(_currentUserService.UserId(), cancellationToken);
        return profile?.Id ?? Guid.Empty;
    }
}