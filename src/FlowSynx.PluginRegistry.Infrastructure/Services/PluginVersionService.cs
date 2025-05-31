using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Tag;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class PluginVersionService : IPluginVersionService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<PluginService> _logger;

    public PluginVersionService(
        IDbContextFactory<ApplicationContext> appContextFactory, 
        ILogger<PluginService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PluginVersionEntity>> AllByPliginId(
        Guid pluginId, 
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            IQueryable<PluginVersionEntity> pluginVersionEntities = context.PluginVersions
                .Include(i => i.PluginVersionTags)
                .Include(i => i.Plugin).ThenInclude(i => i.Owners).ThenInclude(i => i.Profile)
                .Include(i => i.PluginVersionTags).ThenInclude(i => i.Tag)
                .Where(p => !p.IsDeleted && p.Plugin != null);


            return await pluginVersionEntities
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PluginVersionEntity?> GetByPluginType(
        string pluginType, 
        string pluginVersion, 
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            IQueryable<PluginVersionEntity> pluginVersionEntities = context.PluginVersions
                .Include(i => i.PluginVersionTags)
                .Include(i => i!.Statistics)
                .Include(i => i.Plugin).ThenInclude(i=>i.Owners).ThenInclude(i => i.Profile)
                .Include(i => i.PluginVersionTags).ThenInclude(i => i.Tag)
                .Where(p => !p.IsDeleted && p.Plugin != null && p.Version == pluginVersion && p.Plugin.Type == pluginType);

            return await pluginVersionEntities
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(PluginVersionEntity pluginVersionEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            await context.PluginVersions
                .AddAsync(pluginVersionEntity, cancellationToken)
                .ConfigureAwait(false);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task AddTagsToPluginVersionAsync(
        Guid pluginVersionId,
        List<string> tagNames,
        CancellationToken cancellationToken)
    {
        var normalizedTagNames = tagNames
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!normalizedTagNames.Any())
            return;

        await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
        var pluginVersion = await context.PluginVersions
            .Include(pv => pv.PluginVersionTags)
            .FirstOrDefaultAsync(pv => pv.Id == pluginVersionId);

        if (pluginVersion == null)
            throw new InvalidOperationException("Plugin version not found.");

        var existingTags = await context.Tags
            .Where(t => normalizedTagNames.Contains(t.Name))
            .ToListAsync();

        var existingTagNames = existingTags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newTagNames = normalizedTagNames
            .Where(t => !existingTagNames.Contains(t))
            .ToList();

        var newTags = newTagNames.Select(name => new TagEntity { Id = Guid.NewGuid(), Name = name }).ToList();
        await context.Tags.AddRangeAsync(newTags);

        var allTags = existingTags.Concat(newTags).ToList();

        foreach (var tag in allTags)
        {
            if (!pluginVersion.PluginVersionTags.Any(pvt => pvt.TagId == tag.Id))
            {
                pluginVersion.PluginVersionTags.Add(new PluginVersionTagEntity
                {
                    TagId = tag.Id,
                    PluginVersionId = pluginVersion.Id
                });
            }
        }

        await context
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> Delete(
        PluginVersionEntity pluginEntity,
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            context.PluginVersions.Remove(pluginEntity);
            context.SoftDelete(pluginEntity);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}