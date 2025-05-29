using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginLocation;

public class PluginLocationValidator : AbstractValidator<PluginLocationRequest>
{
    public PluginLocationValidator()
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