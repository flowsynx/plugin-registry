using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlowSynx.PluginRegistry.Application.Configuration;

namespace FlowSynx.Pluginregistry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCancellationTokenSource(this IServiceCollection services)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        services.AddSingleton(cancellationTokenSource);
        return services;
    }

    public static IServiceCollection AddGitHubAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationConfiguration = new AuthenticationConfiguration();
        configuration.GetSection("Authentication").Bind(authenticationConfiguration);
        services.AddSingleton(authenticationConfiguration);

        if (authenticationConfiguration.GitHub is null)
            throw new Exception("The GitHub authentication is not initilized!");
        
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "GitHub";
        })
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(authenticationConfiguration.GitHub.CookieExpireTimeSpanMinutes);
            options.SlidingExpiration = true;
        })
        .AddOAuth("GitHub", options =>
        {
            options.ClientId = authenticationConfiguration.GitHub.ClientId;
            options.ClientSecret = authenticationConfiguration.GitHub.ClientSecret;
            options.CallbackPath = authenticationConfiguration.GitHub.CallbackPath;
            options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            options.UserInformationEndpoint = "https://api.github.com/user";
            options.SaveTokens = true;
            options.Scope.Add("user:email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey("urn:github:login", "login");

            options.Events.OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue("FlowSynxPluginRegistry", "1.0"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);

                var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                emailRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("FlowSynxPluginRegistry", "1.0"));
                emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var emailResponse = await context.Backchannel.SendAsync(emailRequest);
                emailResponse.EnsureSuccessStatusCode();

                using var emails = JsonDocument.Parse(await emailResponse.Content.ReadAsStringAsync());

                if (emails.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var primaryEmail = emails.RootElement.EnumerateArray()
                        .FirstOrDefault(e =>
                            e.TryGetProperty("primary", out var p) && p.GetBoolean() &&
                            e.TryGetProperty("verified", out var v) && v.GetBoolean());

                    if (primaryEmail.ValueKind != JsonValueKind.Undefined &&
                        primaryEmail.TryGetProperty("email", out var emailProp))
                    {
                        var email = emailProp.GetString();
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email));
                        }
                    }
                }
            };

            options.Events.OnRedirectToAuthorizationEndpoint = context =>
            {
                var prompt = "&prompt=login";
                context.Response.Redirect(context.RedirectUri + prompt);
                return Task.CompletedTask;
            };
        });

        return services;
    }

    public static IServiceCollection AddEndpoint(this IServiceCollection services, IConfiguration configuration)
    {
        var endpointConfiguration = new EndpointConfiguration();
        configuration.GetSection("Endpoint").Bind(endpointConfiguration);
        services.AddSingleton(endpointConfiguration);
        return services;
    }

    public static IServiceCollection AddHttpJsonOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}