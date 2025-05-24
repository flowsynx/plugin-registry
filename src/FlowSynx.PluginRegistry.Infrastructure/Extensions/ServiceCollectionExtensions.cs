using FlowSynx.PluginRegistry.Application.Services;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowSynx.PluginRegistry.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection"); 

        services
            .AddSingleton<ISystemClock, SystemClock>()
            .AddScoped<IPluginService, PluginService>()
            .AddScoped<IPluginVersionService, PluginVersionService>()
            .AddScoped<IGravatar, Gravatar>()
            .AddDbContextFactory<ApplicationContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        return services;
    }
}