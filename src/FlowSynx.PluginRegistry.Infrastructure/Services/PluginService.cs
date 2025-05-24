using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class PluginService : IPluginService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<PluginService> _logger;

    public PluginService(IDbContextFactory<ApplicationContext> appContextFactory, ILogger<PluginService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PluginEntity>> All(CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var result = await context.Plugins
                .Where(p => p.IsDeleted == false)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IReadOnlyCollection<PluginEntity>> AllBySeachQuery(
        string? query,
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            IQueryable<PluginEntity> pluginEntities = context.Plugins.Include(i=>i.Owners).ThenInclude(i=>i.Profile);

            if (!string.IsNullOrEmpty(query))
            {
                pluginEntities = pluginEntities.Where(p => (EF.Functions.ILike(p.Type, $"%{query}%") ||
                    EF.Functions.ILike(p.LatestDescription ?? string.Empty, $"%{query}%") || 
                    EF.Functions.ILike(p.LatestVersion, $"%{query}%") ||
                    EF.Functions.Like(p.LatestTags, $"%{query};%") ||
                    EF.Functions.Like(p.LatestTags, $"%{query}") ||
                    EF.Functions.Like(p.LatestTags, $"{query};%") ||
                    p.LatestTags == query)
                    && p.IsDeleted == false);
            }

            return await pluginEntities.ToListAsync(cancellationToken).ConfigureAwait(false);
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
            using var context = _appContextFactory.CreateDbContext();
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

    public async Task<bool> Delete(PluginEntity pluginEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
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