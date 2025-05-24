namespace FlowSynx.PluginRegistry.Domain.Profile;

public interface IProfileService
{
    Task<ProfileEntity?> GetByProfileId(string profileId, CancellationToken cancellationToken);
    Task Add(ProfileEntity profileEntity, CancellationToken cancellationToken);
    Task<bool> Delete(ProfileEntity profileEntity, CancellationToken cancellationToken);
}