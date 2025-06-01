using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginIcon;

public class PluginIconValidator : AbstractValidator<PluginIconRequest>
{
    public PluginIconValidator()
    {
        RuleFor(x => x.PluginType)
            .NotNull()
            .NotEmpty()
            .WithMessage("Plugin type must have value!");

        RuleFor(x => x.PluginVersion)
            .NotNull()
            .NotEmpty()
            .WithMessage("Plugin version must have value!");
    }
}