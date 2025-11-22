using System.Net.Http.Headers;

namespace FlowSynx.Pluginregistry.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationMessageHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Get cookies from the current HTTP context
            var cookies = httpContext.Request.Headers.Cookie;
            
            if (!string.IsNullOrEmpty(cookies))
            {
                request.Headers.Add("Cookie", cookies.ToString());
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}