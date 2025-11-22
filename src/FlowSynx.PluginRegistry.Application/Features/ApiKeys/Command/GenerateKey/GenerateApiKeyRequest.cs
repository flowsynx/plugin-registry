using FlowSynx.PluginRegistry.Application.Wrapper;
using MediatR;

namespace FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;

public class GenerateApiKeyRequest : IRequest<Result<GenerateApiKeyResponse>>
{
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool CanPushNewPlugins { get; set; } = false;
    public bool CanPushPluginVersions { get; set; } = true;
    public List<Guid> PluginIds { get; set; } = new();
}