using FlowSynx.PluginRegistry.Application.Services;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Profile;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;

namespace FlowSynx.PluginRegistry.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DatabaseConnection");
        var normalizePostgresConnectionString = NormalizePostgresConnectionString(connectionString);

        services
            .AddSingleton<ISystemClock, SystemClock>()
            .AddScoped<IPluginService, PluginService>()
            .AddScoped<IPluginVersionService, PluginVersionService>()
            .AddScoped<IProfileService, ProfileService>()
            .AddScoped<IGravatar, Gravatar>()
            .AddDbContextFactory<ApplicationContext>(options =>
            {
                options.UseNpgsql(normalizePostgresConnectionString);
            });
        return services;
    }

    public static string NormalizePostgresConnectionString(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new Exception("Database connectionstring is required!");

        if (input.StartsWith("postgres://") || input.StartsWith("postgresql://"))
        {
            var uri = new Uri(input);
            var userInfo = uri.UserInfo.Split(':', 2);
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = uri.Host;
            var port = uri.Port == -1 ? 5432 : uri.Port;

            return new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                Database = uri.AbsolutePath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            }.ConnectionString;
        }

        return input;
    }
}