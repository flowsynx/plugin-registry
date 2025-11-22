using System.Security.Claims;
using FlowSynx.PluginRegistry.Application.Services;

namespace FlowSynx.Pluginregistry.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly HttpContext? _httpContext;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor, 
        ILogger<CurrentUserService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public string UserId()
    {
        try
        {
            return _httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw CreateException(ex);
        }
    }

    /// <inheritdoc />
    public string UserName()
    {
        try
        {
            return _httpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw CreateException(ex);
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated()
    {
        try
        {
            var identity = _httpContext?.User.Identity;
            return identity is { IsAuthenticated: true };
        }
        catch (Exception ex)
        {
            throw CreateException(ex);
        }
    }

    /// <inheritdoc />
    public List<string> Roles()
    {
        try
        {
            var user = _httpContext?.User;
            if (user == null)
                return new List<string>();

            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return roles;
        }
        catch (Exception ex)
        {
            throw CreateException(ex);
        }
    }

    /// <inheritdoc />
    public void ValidateAuthentication()
    {
        if (string.IsNullOrEmpty(UserId()))
        {
            throw new UnauthorizedAccessException("Authentication Access Denied");
        }
    }

    private Exception CreateException(Exception exception)
    {
        _logger.LogError(exception.ToString());
        return new InvalidOperationException(exception.Message);
    }
}
