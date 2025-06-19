using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Domain.Plugin;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class PluginCategoryService : IPluginCategoryService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<PluginService> _logger;

    public PluginCategoryService(
        IDbContextFactory<ApplicationContext> appContextFactory, 
        ILogger<PluginService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<PluginCategoryEntity>> All(CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            IQueryable<PluginCategoryEntity> pluginVersionEntities = context.PluginCategories;

            return await pluginVersionEntities
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PluginCategoryEntity?> GetByCategoryId(string categoryId, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            IQueryable<PluginCategoryEntity> pluginVersionEntities = context.PluginCategories
                .Where(p => p.CategoryId.ToLower() == categoryId.ToLower());

            return await pluginVersionEntities
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}