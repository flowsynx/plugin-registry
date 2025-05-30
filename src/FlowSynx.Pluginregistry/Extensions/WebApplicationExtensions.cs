using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.PluginRegistry.Application.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;

namespace FlowSynx.Pluginregistry.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigRedirection(this WebApplication app)
    {
        var endpointConfiguration = app.Services.GetRequiredService<EndpointConfiguration>();

        if (endpointConfiguration.HttpsRedirection is true)
            app.UseHttpsRedirection();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        return app;
    }

    public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GetType().Name;

        return app
            .MapGroup($"/api/{groupName}")
            .WithTags(groupName);
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);
        var assembly = Assembly.GetExecutingAssembly();
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
                instance.Map(app);
        }

        return app;
    }
}