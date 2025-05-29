namespace FlowSynx.PluginRegistry.Domain.Profile;

public interface IProfileService
{
    Task<ProfileEntity?> GetByUserId(string userId, CancellationToken cancellationToken);
    Task Add(ProfileEntity profileEntity, CancellationToken cancellationToken);
    Task<bool> Delete(ProfileEntity profileEntity, CancellationToken cancellationToken);
}