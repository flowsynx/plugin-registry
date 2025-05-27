using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.Pluginregistry.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FlowSynx.Endpoints;

public class Account : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("/login", Login)
            .WithName("Login");

        group.MapGet("/logout", Logout)
            .WithName("Logout");
    }

    public async Task Login(HttpContext context, CancellationToken cancellationToken)
    {
        var returnUrl = context.Request.Query["returnUrl"].ToString() ?? "/";
        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        await context.ChallengeAsync("GitHub", props);
    }

    public async Task Logout(HttpContext context, CancellationToken cancellationToken)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/");
    }
}