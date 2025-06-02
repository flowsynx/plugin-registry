using FlowSynx.Pluginregistry.Endpoints;
using FlowSynx.Pluginregistry.Extensions;
using FlowSynx.PluginRegistry.Application.Extensions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FlowSynx.Pluginregistry.Services;
using Microsoft.AspNetCore.StaticFiles;
using static System.Net.Mime.MediaTypeNames;

namespace FlowSynx.Endpoints;

public class Plugins : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapGet("", PluginsListAsync)
            .WithName("PluginsList");

        group.MapGet("/{type}/{version}", GetPluginWithTypeAsync)
            .WithName("GetPluginWithType");

        group.MapGet("/{type}/{version}/icon", GetPluginIconWithTypeAsync)
            .WithName("GetPluginIconWithType");

        group.MapGet("/{type}/{version}/readme", GetPluginReadmeWithTypeAsync)
            .WithName("GetPluginReadmeWithType");

        group.MapGet("/{type}/{version}/download", DownloadPluginByTypeAsync)
            .WithName("DownloadPluginByType");
    }

    public async Task<IResult> PluginsListAsync([FromQuery] string? q, [FromQuery] int? page, 
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.PluginsList(q, page, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }

    public async Task<IResult> GetPluginWithTypeAsync(string type, string version, [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.PluginDetails(type, version, cancellationToken);
        return result.Succeeded ? Results.Ok(result) : Results.NotFound(result);
    }

    public async Task<IResult> GetPluginIconWithTypeAsync(string type, string version, [FromServices] IMediator mediator,
       IFileStorage fileStorage,  CancellationToken cancellationToken)
    {
        var defaultIconPath = Path.Combine("wwwroot", "images", "NoPluginIcon.png");
        var result = await mediator.PluginIcon(type, version, cancellationToken);

        if (!result.Succeeded)
        {
            var fileBytes = await File.ReadAllBytesAsync(defaultIconPath);
            return Results.File(fileBytes, "image/png");
        }

        if (string.IsNullOrEmpty(result.Data.Icon)) 
        {
            var fileBytes = await File.ReadAllBytesAsync(defaultIconPath);
            return Results.File(fileBytes, "image/png");
        }
            
        var fileExist = await fileStorage.FileExistsAsync(result.Data.Icon);
        if (!fileExist)
        {
            var fileBytes = await File.ReadAllBytesAsync(defaultIconPath);
            return Results.File(fileBytes, "image/png");
        }
        
        var iconToServe = await fileStorage.ReadFileAsync(result.Data.Icon);
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(result.Data.Icon, out var contentType))
            contentType = "application/octet-stream";

        return Results.File(iconToServe, contentType);
    }

    public async Task<IResult> GetPluginReadmeWithTypeAsync(string type, string version, [FromServices] IMediator mediator,
        IFileStorage fileStorage, CancellationToken cancellationToken)
    {
        var result = await mediator.PluginReadme(type, version, cancellationToken);
        var defaultReadme = result.Data.Description;

        if (!result.Succeeded)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("No README defined");
            return Results.File(bytes, "text/markdown");
        }

        if (string.IsNullOrEmpty(result.Data.Readme))
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(result.Data.Description))
                bytes = System.Text.Encoding.UTF8.GetBytes(result.Data.Description);
            else
                bytes = System.Text.Encoding.UTF8.GetBytes("No README defined");
            
            return Results.File(bytes, "text/markdown");
        }

        var fileExist = await fileStorage.FileExistsAsync(result.Data.Readme);
        if (!fileExist)
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(result.Data.Description))
                bytes = System.Text.Encoding.UTF8.GetBytes(result.Data.Description);
            else
                bytes = System.Text.Encoding.UTF8.GetBytes("No README defined");

            return Results.File(bytes, "text/markdown");
        }

        var iconToServe = await fileStorage.ReadFileAsync(result.Data.Readme);
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(result.Data.Readme, out var contentType))
            contentType = "application/octet-stream";

        return Results.File(iconToServe, contentType);
    }

    public async Task DownloadPluginByTypeAsync(
        string type,
        string version,
        HttpContext context,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.PluginLocation(type, version, cancellationToken);
        if (!result.Succeeded)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Plugin not found", cancellationToken);
            return;
        }

        var pluginLocation = result.Data.PluginLocation;
        if (!File.Exists(pluginLocation))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("File not found", cancellationToken);
            return;
        }

        var checksum = result.Data.Checksum;
        var fileName = Path.GetFileName(pluginLocation);
        var contentType = "application/octet-stream"; // safe default for unknown binary types

        // Set headers BEFORE writing response body
        context.Response.Clear();
        context.Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
        context.Response.ContentType = contentType;
        context.Response.Headers.Append("X-Checksum", checksum);

        // Stream the file manually
        await using var fileStream = new FileStream(pluginLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
        await fileStream.CopyToAsync(context.Response.Body, cancellationToken);
    }
}