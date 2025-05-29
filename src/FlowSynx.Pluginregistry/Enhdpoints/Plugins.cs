using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.PluginRegistry.Application.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowSynx.Endpoints;

public class Plugins : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("", PluginsList)
            .WithName("PluginsList");

        group.MapGet("/{type}/{version}", GetPluginWithType)
            .WithName("GetPluginWithType");
    }

    public async Task<IResult> PluginsList([FromQuery] string? q, [FromQuery] int? page, 
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.PluginsList(q, page, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }

    public async Task<IResult> GetPluginWithType(string? type, string version, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.PluginDetails(type, version, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }
}