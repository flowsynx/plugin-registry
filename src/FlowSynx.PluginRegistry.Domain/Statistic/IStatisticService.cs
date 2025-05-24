using FlowSynx.PluginRegistry.Domain.Statistic;

namespace FlowSynx.PluginRegistry.Domain.Plugin;

public interface IStatisticService
{
    Task<int> GetDownloadCount(Guid pluginVersionId, CancellationToken cancellationToken);
    Task Add(StatisticEntity statisticEntity, CancellationToken cancellationToken);
}