using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Infrastructure.Extensions;
using FlowSynx.PluginRegistry.Domain.Plugin;

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