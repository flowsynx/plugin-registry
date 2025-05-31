using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.PluginRegistry.Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace FlowSynx.Endpoints;

public class Profiles : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("/{UserName}", PluginByUsername)
            .WithName("PluginByUsername");

        group.MapGet("/{UserName}/statistics", GetPluginStatisticsByUsernameAsync)
            .WithName("PluginStatisticsByUsername");
    }

    public async Task<IResult> PluginByUsername(string userName, [FromQuery] int? page, 
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.PluginsListByuserName(userName, page, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }

    public async Task<IResult> GetPluginStatisticsByUsernameAsync(string userName, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.PluginStatisticsByUsernameAsync(userName, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }
}