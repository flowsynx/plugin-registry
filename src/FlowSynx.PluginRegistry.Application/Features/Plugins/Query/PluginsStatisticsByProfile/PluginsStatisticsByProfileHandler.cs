using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Profile;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;

internal class PluginsStatisticsByProfileHandler : IRequestHandler<PluginsStatisticsByProfileRequest, Result<PluginsStatisticsByProfileResponse>>
{
    private readonly ILogger<PluginsStatisticsByProfileHandler> _logger;
    private readonly IProfileService _profileService;

    public PluginsStatisticsByProfileHandler(
        ILogger<PluginsStatisticsByProfileHandler> logger,
        IProfileService profileService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(profileService);
        _logger = logger;
        _profileService = profileService;
    }

    public async Task<Result<PluginsStatisticsByProfileResponse>> Handle(PluginsStatisticsByProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _profileService.GetPluginStatisticsByUsernameAsync(request.UserName, cancellationToken);

            var response = new PluginsStatisticsByProfileResponse
            {
                Email = result.ProfileEmail,
                TotalCount = result.PluginCount,
                TotalDownload = result.DownloadCount
            };

            return await Result<PluginsStatisticsByProfileResponse>.SuccessAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<PluginsStatisticsByProfileResponse>.FailAsync(ex.ToString());
        }
    }
}