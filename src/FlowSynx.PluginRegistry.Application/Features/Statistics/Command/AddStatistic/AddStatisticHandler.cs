using FlowSynx.PluginRegistry.Application.Wrapper;
using FlowSynx.PluginRegistry.Domain.Plugin;
using FlowSynx.PluginRegistry.Domain.Statistic;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowSynx.PluginRegistry.Application.Features.Statistics.Command.AddStatistic;

internal class AddStatisticHandler : IRequestHandler<AddStatisticRequest, Result<Unit>>
{
    private readonly ILogger<AddStatisticHandler> _logger;
    private readonly IPluginVersionService _versionService;
    private readonly IStatisticService _statisticService;

    public AddStatisticHandler(
        ILogger<AddStatisticHandler> logger,
        IPluginVersionService versionService,
        IStatisticService statisticService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        _versionService = versionService;
        _statisticService = statisticService;
    }

    public async Task<Result<Unit>> Handle(
        AddStatisticRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var plugin = await _versionService.GetByPluginType(request.PluginType, request.PluginVersion, cancellationToken);
            if (plugin is null)
                throw new Exception($"Plugin details for '{request.PluginType}' v{request.PluginVersion} could not be found");

            var response = new StatisticEntity
            {
                Id = Guid.NewGuid(),
                PluginId = plugin.PluginId,
                PluginVersionId = plugin.Id,
                IPAddress = request.IPAddress,
                UserAgent = request.UserAgent
            };

            await _statisticService.Add(response, cancellationToken);
            return await Result<Unit>.SuccessAsync("Statistics updated!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return await Result<Unit>.FailAsync(ex.ToString());
        }
    }
}