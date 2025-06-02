using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.Statistics.Command.AddStatistic;

public class AddStatisticRequest : IRequest<Result<Unit>>
{
    public required string PluginType { get; set; }
    public required string PluginVersion { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
}