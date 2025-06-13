using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginVersions;

public class PluginVersionsValidator : AbstractValidator<PluginVersionsRequest>
{
    public PluginVersionsValidator()
    {
        RuleFor(x => x.PluginType)
            .NotNull()
            .NotEmpty()
            .WithMessage("Plugin type must have value!");
    }
}