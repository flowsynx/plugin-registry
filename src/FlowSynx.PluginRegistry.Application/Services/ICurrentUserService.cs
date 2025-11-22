namespace FlowSynx.PluginRegistry.Application.Services;

public interface ICurrentUserService
{
    string UserId();
    string UserName();
    bool IsAuthenticated();
    List<string> Roles();
    void ValidateAuthentication();
}
