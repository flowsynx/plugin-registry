using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Domain.Profile;

public interface IProfileService
{
    Task<ProfileEntity?> GetByUserId(string userId, CancellationToken cancellationToken);
    Task<(int PluginCount, int DownloadCount, string? ProfileEmail)> GetPluginStatisticsByUsernameAsync(string username, 
        CancellationToken cancellationToken = default);
    Task Add(ProfileEntity profileEntity, CancellationToken cancellationToken);
    Task<bool> Delete(ProfileEntity profileEntity, CancellationToken cancellationToken);
}