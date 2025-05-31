using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Plugins.Query.PluginsListByProfile;

public class PluginsListByProfileValidator : AbstractValidator<PluginsListByProfileRequest>
{
    public PluginsListByProfileValidator()
    {
        RuleFor(x => x.UserName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Profile username must have value!");
    }
}