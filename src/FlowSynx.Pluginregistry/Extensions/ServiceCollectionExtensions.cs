using FlowSynx.Pluginregistry.Services;
using FlowSynx.PluginRegistry.Application.Configuration;
using FlowSynx.PluginRegistry.Domain.Profile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowSynx.Pluginregistry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCancellationTokenSource(this IServiceCollection services)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        services.AddSingleton(cancellationTokenSource);
        return services;
    }

    public static IServiceCollection AddBaseAddress(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("Api", (sp, client) =>
        {
            var baseUri = ResolveBaseUri(sp);
            client.BaseAddress = new Uri(baseUri);
        })
        .AddHttpMessageHandler<AuthorizationMessageHandler>();

        services.AddScoped<AuthorizationMessageHandler>();

        return services;
    }

    private static string ResolveBaseUri(IServiceProvider serviceProvider)
    {
        var endpoint = serviceProvider.GetRequiredService<EndpointConfiguration>();
        return endpoint.BaseAddress ?? "http://localhost:7236/";
    }

    public static IServiceCollection AddDataProtectionService(this IServiceCollection services)
    {
        services.AddDataProtection()
            .SetApplicationName("PluginRegistryApp")
            .PersistKeysToFileSystem(new DirectoryInfo("/app/dpkeys"));

        return services;
    }
    public static IServiceCollection AddGitHubAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationConfiguration = new AuthenticationConfiguration();
        configuration.GetSection("Authentication").Bind(authenticationConfiguration);
        services.AddSingleton(authenticationConfiguration);

        if (authenticationConfiguration.GitHub is null)
            throw new Exception("The GitHub authentication is not initilized!");

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Lax;
        });

        services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme, options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

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
            options.CallbackPath = new PathString(authenticationConfiguration.GitHub.CallbackPath);
            options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
            options.TokenEndpoint = "https://github.com/login/oauth/access_token";
            options.UserInformationEndpoint = "https://api.github.com/user";
            options.SaveTokens = true;
            options.Scope.Add("user:email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey("login", "login");

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
                string? email = "";
                if (emails.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var primaryEmail = emails.RootElement.EnumerateArray()
                        .FirstOrDefault(e =>
                            e.TryGetProperty("primary", out var p) && p.GetBoolean() &&
                            e.TryGetProperty("verified", out var v) && v.GetBoolean());

                    if (primaryEmail.ValueKind != JsonValueKind.Undefined &&
                        primaryEmail.TryGetProperty("email", out var emailProp))
                    {
                        email = emailProp.GetString();
                        if (!string.IsNullOrEmpty(email))
                        {
                            context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email));
                        }
                    }
                }

                var profileService = context.HttpContext.RequestServices.GetRequiredService<IProfileService>();

                var githubId = user.RootElement.GetProperty("id").GetInt64();
                var login = user.RootElement.GetProperty("login").GetString();
                var userName = user.RootElement.GetProperty("name").GetString();
                var profileUrl = user.RootElement.GetProperty("html_url").GetString();

                var existingUser = await profileService.GetByUserId(githubId.ToString(), CancellationToken.None);
                if (existingUser == null)
                {
                    await profileService.Add(new ProfileEntity
                    {
                        Id = Guid.NewGuid(),
                        UserId = githubId.ToString(),
                        UserName = login,
                        Email = email
                    }, CancellationToken.None);
                }
            };

            options.Events.OnRedirectToAuthorizationEndpoint = context =>
            {
                var redirectUri = context.RedirectUri;

                if (!redirectUri.Contains("prompt=login", StringComparison.OrdinalIgnoreCase))
                {
                    var separator = redirectUri.Contains("?") ? "&" : "?";
                    redirectUri += $"{separator}prompt=login";
                }

                context.Response.Redirect(redirectUri);
                return Task.CompletedTask;
            };

            options.Events.OnRemoteFailure = context =>
            {
                // Optional: log error details
                var error = context.Failure?.Message;

                // Redirect the user to a friendly error page or login page
                context.Response.Redirect("/api/account/login?error=access_denied");
                context.HandleResponse(); // Prevents the exception from bubbling up
                return Task.CompletedTask;
            };
        });

        return services;
    }

    public static IServiceCollection AddEndpointConfiguration(this IServiceCollection services, IConfiguration configuration)
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