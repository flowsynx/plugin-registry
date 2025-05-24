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
            var result = await context.PluginVersions
                .Where(p => p.PluginId == pluginId && p.IsDeleted == false)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PluginVersionEntity?> GetByPluginId(
        Guid pluginVersionId, 
        CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var result = await context.PluginVersions
                .FirstOrDefaultAsync(p => p.Id == pluginVersionId && p.IsDeleted == false, cancellationToken)
                .ConfigureAwait(false);

            return result;
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
            return await context.PluginVersions
                .Include(i=>i.Plugin)
                .ThenInclude(i=>i.Owners).ThenInclude(i=>i.Profile)
                .Include(i=>i.Statistics)
                .FirstOrDefaultAsync(p => p.Version == pluginVersion && p.Plugin.Type == pluginType && !p.IsDeleted, cancellationToken)
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