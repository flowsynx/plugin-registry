using FlowSynx.Pluginregistry.Components;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Application.Extensions;
using FlowSynx.Pluginregistry.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpsPolicy;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

var customConfigPath = builder.Configuration["config"];
if (!string.IsNullOrEmpty(customConfigPath))
{
    builder.Configuration.Sources.Clear();
    builder.Configuration.AddJsonFile(customConfigPath, optional: false, reloadOnChange: false);
}

IConfiguration config = builder.Configuration;

builder.Services
       .AddHttpContextAccessor()
       .AddEndpoint(config)
       .AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();

builder.Services
       .AddPostgresPersistenceLayer(config)
       .AddApplication();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
});

builder.Services.AddScoped<IStatsApiService, StatsApiService>();
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<IFileValidator, FileValidator>();
builder.Services.AddScoped<IPluginMetadataReader, PluginMetadataReader>();

builder.Services.AddGitHubAuthentication(config);
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddDataProtectionService();

builder.ConfigHttpServer();
builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = 443;
});
builder.Services.AddBaseAddress(config);

var app = builder.Build();
app.ConfigRedirection();

app.UseExceptionHandler("/Error", createScopeForErrors: true);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.EnsureApplicationDatabaseCreated();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();