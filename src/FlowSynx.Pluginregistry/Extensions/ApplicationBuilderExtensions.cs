using FlowSynx.PluginRegistry.Infrastructure.Contexts;

namespace FlowSynx.Pluginregistry.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder EnsureApplicationDatabaseCreated(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var result = context.Database.EnsureCreated();
            if (result)
                logger.LogInformation("Application database created successfully.");
            else
                logger.LogInformation("Application database already exists.");

            return app;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while creating the application database: {ex.Message}");
        }
    }
}