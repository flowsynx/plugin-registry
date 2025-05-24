using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginDetails;

public class PluginDetailsValidator : AbstractValidator<PluginDetailsRequest>
{
    public PluginDetailsValidator()
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