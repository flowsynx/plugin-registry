using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class PluginService : IPluginService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<PluginService> _logger;
    private readonly int _pageSize = 20;

    public PluginService(IDbContextFactory<ApplicationContext> appContextFactory, ILogger<PluginService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<Pagination<PluginEntity>> All(int page, CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<PluginEntity> pluginEntities = context.Plugins
                .Include(i => i.LatestVersion).ThenInclude(i => i!.PluginVersionTags)
                .Include(i => i.Owners).ThenInclude(i => i.Profile)
                .Include(i => i.Versions).ThenInclude(i => i.PluginVersionTags).ThenInclude(i => i.Tag)
                .Where(p => !p.IsDeleted && p.LatestVersion != null);

            var totalCount = await pluginEntities.CountAsync(cancellationToken).ConfigureAwait(false);

            var skip = (page - 1) * _pageSize;
            pluginEntities = pluginEntities.OrderBy(p => p.Type)
                .Skip(skip)
                .Take(_pageSize);

            var result = await pluginEntities.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Pagination<PluginEntity>(result, totalCount, page, _pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PluginEntity?> GetByPluginType(
        string pluginType,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<PluginEntity> pluginEntity = context.Plugins
                .Include(i => i.Versions)
                .Where(p => !p.IsDeleted);

            return await pluginEntity
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Pagination<PluginEntity>> AllBySeachQuery(
        string? query,
        int page,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<PluginEntity> pluginEntities = context.Plugins
                .Include(i => i.LatestVersion).ThenInclude(i => i!.PluginVersionTags)
                .Include(i => i.Owners).ThenInclude(i => i.Profile)
                .Include(i => i.Versions).ThenInclude(i => i.PluginVersionTags).ThenInclude(i => i.Tag)
                .Where(p => !p.IsDeleted && p.LatestVersion != null);

            if (!string.IsNullOrEmpty(query))
            {
                pluginEntities = pluginEntities.Where(p => EF.Functions.ILike(p.Type, $"%{query}%") ||
                    EF.Functions.ILike(p.LatestVersion!.Description ?? string.Empty, $"%{query}%") ||
                    EF.Functions.ILike(p.LatestVersion!.Version, $"%{query}%") ||
                    p.LatestVersion!.PluginVersionTags.Any(t =>
                    EF.Functions.ILike(t.Tag!.Name, $"%{query}%")));
            }

            var totalCount = await pluginEntities.CountAsync(cancellationToken).ConfigureAwait(false);

            var skip = (page - 1) * _pageSize;
            pluginEntities = pluginEntities.OrderBy(p => p.Type)
                .Skip(skip)
                .Take(_pageSize);

            var result = await pluginEntities.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Pagination<PluginEntity>(result, totalCount, page, _pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Pagination<PluginEntity>> AllBySeachTags(
        string? tag,
        int page,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<PluginEntity> pluginEntities = context.Plugins
                .Include(i => i.LatestVersion).ThenInclude(i => i!.PluginVersionTags)
                .Include(i => i.Owners).ThenInclude(i => i.Profile)
                .Include(i => i.Versions).ThenInclude(i => i.PluginVersionTags).ThenInclude(i => i.Tag)
                .Where(p => !p.IsDeleted && p.LatestVersion != null);

            if (!string.IsNullOrEmpty(tag))
            {
                pluginEntities = pluginEntities.Where(p => p.LatestVersion!.PluginVersionTags.Any(t =>
                    EF.Functions.ILike(t.Tag!.Name, $"%{tag}%")));
            }

            var totalCount = await pluginEntities.CountAsync(cancellationToken).ConfigureAwait(false);

            var skip = (page - 1) * _pageSize;
            pluginEntities = pluginEntities.OrderBy(p => p.Type)
                .Skip(skip)
                .Take(_pageSize);

            var result = await pluginEntities.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new Pagination<PluginEntity>(result, totalCount, page, _pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(PluginEntity pluginEntity, CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Plugins
                .AddAsync(pluginEntity, cancellationToken)
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

    public async Task Update(PluginEntity pluginEntity, CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            context.Entry(pluginEntity).State = EntityState.Detached;
            context.Plugins.Update(pluginEntity);

            await context
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> Delete(PluginEntity pluginEntity, CancellationToken cancellationToken)
    {
        try
        {
            await using var context = await _appContextFactory.CreateDbContextAsync(cancellationToken);
            context.Plugins.Remove(pluginEntity);
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