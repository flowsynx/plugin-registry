using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsStatisticsByProfile;

public class PluginsStatisticsByProfileValidator : AbstractValidator<PluginsStatisticsByProfileRequest>
{
    public PluginsStatisticsByProfileValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Profile username must have value!");
    }
}