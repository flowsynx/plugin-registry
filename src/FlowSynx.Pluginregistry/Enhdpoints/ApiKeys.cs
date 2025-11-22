using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.Pluginregistry.Models;
using FlowSynx.PluginRegistry.Application.Extensions;
using FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;
using FlowSynx.PluginRegistry.Domain.ApiKey;
using FlowSynx.PluginRegistry.Domain.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace FlowSynx.PluginRegistry.Endpoints;

public class ApiKeys : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("/", GetApiKeys)
            .WithName("GetApiKeys")
            .RequireAuthorization();

        group.MapPost("/", GenerateApiKey)
            .WithName("GenerateApiKey")
            .RequireAuthorization();

        group.MapDelete("/{keyId:guid}", RevokeApiKey)
            .WithName("RevokeApiKey")
            .RequireAuthorization();
    }

    private static async Task<IResult> GetApiKeys(
        ClaimsPrincipal user,
        IApiKeyService apiKeyService,
        IProfileService profileService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var profileId = await GetCurrentUserProfileId(user, profileService, cancellationToken);
            if (profileId == Guid.Empty)
                return Results.Json(
                    await Application.Wrapper.Result<List<ApiKeyViewModel>>.FailAsync("User profile not found"),
                    statusCode: 404);

            var apiKeys = await apiKeyService.GetByUserId(profileId, cancellationToken);
            
            var viewModels = apiKeys?.Select(k => new ApiKeyViewModel
            {
                Id = k.Id,
                Name = k.Name,
                Key = k.Key,
                CreatedAt = k.CreatedOn,
                ExpiresAt = k.ExpiresAt,
                IsRevoked = k.IsRevoked,
                CanPushNewPlugins = k.CanPushNewPlugins,
                CanPushPluginVersions = k.CanPushPluginVersions,
                AssignedPlugins = k.PluginAssignments?.Select(p => p.PluginId.ToString()).ToList() ?? new()
            }).ToList() ?? new();

            return Results.Json(await Application.Wrapper.Result<List<ApiKeyViewModel>>.SuccessAsync(viewModels));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving API keys");
            return Results.Json(
                await Application.Wrapper.Result<List<ApiKeyViewModel>>.FailAsync($"Error retrieving API keys: {ex.Message}"),
                statusCode: 500);
        }
    }

    private static async Task<IResult> GenerateApiKey(
        [FromBody] GenerateApiKeyRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.GenerateApiKey(request, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> RevokeApiKey(
        Guid keyId,
        ClaimsPrincipal user,
        IApiKeyService apiKeyService,
        IProfileService profileService,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        try
        {
            var profileId = await GetCurrentUserProfileId(user, profileService, cancellationToken);
            if (profileId == Guid.Empty)
                return Results.Json(
                    await Application.Wrapper.Result<bool>.FailAsync("User profile not found"),
                    statusCode: 404);

            var result = await apiKeyService.RevokeKeyAsync(keyId, profileId, cancellationToken);
            
            if (result)
                return Results.Json(await Application.Wrapper.Result<bool>.SuccessAsync(true, "API key revoked successfully"));
            
            return Results.Json(
                await Application.Wrapper.Result<bool>.FailAsync("API key not found or already revoked"),
                statusCode: 404);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking API key");
            return Results.Json(
                await Application.Wrapper.Result<bool>.FailAsync($"Error revoking API key: {ex.Message}"),
                statusCode: 500);
        }
    }

    private static async Task<Guid> GetCurrentUserProfileId(
        ClaimsPrincipal user,
        IProfileService profileService,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Guid.Empty;

        var profile = await profileService.GetByUserId(userId, cancellationToken);
        return profile?.Id ?? Guid.Empty;
    }
}