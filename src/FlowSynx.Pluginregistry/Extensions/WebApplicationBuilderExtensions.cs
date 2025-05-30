using FlowSynx.PluginRegistry.Application.Configuration;

namespace FlowSynx.Pluginregistry.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigHttpServer(this WebApplicationBuilder builder)
    {
        using var scope = builder.Services.BuildServiceProvider().CreateScope();
        var endpointConfiguration = scope.ServiceProvider.GetRequiredService<EndpointConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            builder.WebHost.ConfigureKestrel((_, kestrelOptions) =>
            {
                var httpPort = endpointConfiguration.Http;
                kestrelOptions.ListenAnyIP(httpPort ?? 7236);
            });

            builder.WebHost.UseKestrel(option =>
            {
                option.AddServerHeader = false;
                option.Limits.MaxRequestBufferSize = null;
            });

            return builder;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}