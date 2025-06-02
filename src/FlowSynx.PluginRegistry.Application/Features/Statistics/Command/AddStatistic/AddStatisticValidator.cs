using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.Statistics.Command.AddStatistic;

public class AddStatisticValidator : AbstractValidator<AddStatisticRequest>
{
    public AddStatisticValidator()
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