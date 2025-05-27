using FlowSynx.Pluginregistry.Components;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Application.Extensions;
using FlowSynx.Pluginregistry.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Antiforgery;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

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
       .AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();

builder.Services
       .AddPostgresPersistenceLayer(config)
       .AddApplication();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 50MB
});

builder.Services.AddHttpClient("Api", c =>
{
    c.BaseAddress = new Uri("https://localhost:7236/");
});
builder.Services.AddScoped<IStatsApiService, StatsApiService>();

builder.Services.AddGitHubAuthentication(config);
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.EnsureApplicationDatabaseCreated();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();