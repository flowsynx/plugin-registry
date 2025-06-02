using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FlowSynx.PluginRegistry.Infrastructure.Contexts;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Statistic;

namespace FlowSynx.PluginRegistry.Infrastructure.Services;

public class StatisticService : IStatisticService
{
    private readonly IDbContextFactory<ApplicationContext> _appContextFactory;
    private readonly ILogger<ProfileService> _logger;

    public StatisticService(
        IDbContextFactory<ApplicationContext> appContextFactory, 
        ILogger<ProfileService> logger)
    {
        ArgumentNullException.ThrowIfNull(appContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _appContextFactory = appContextFactory;
        _logger = logger;
    }

    public async Task<int> GetDownloadCount(Guid pluginVersionId, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            var result = await context.Statistics
                .CountAsync(p=>p.PluginVersionId == pluginVersionId, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Add(StatisticEntity statisticEntity, CancellationToken cancellationToken)
    {
        try
        {
            using var context = _appContextFactory.CreateDbContext();
            await context.Statistics
                .AddAsync(statisticEntity, cancellationToken)
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
}