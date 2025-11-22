using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Command.SetPluginVersionActiveStatus;

public class SetPluginVersionActiveStatusValidator : AbstractValidator<SetPluginVersionActiveStatusRequest>
{
    public SetPluginVersionActiveStatusValidator()
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