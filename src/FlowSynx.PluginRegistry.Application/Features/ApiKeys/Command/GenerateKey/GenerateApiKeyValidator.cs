using FluentValidation;

namespace FlowSynx.PluginRegistry.Application.Features.ApiKeys.Command.GenerateKey;

public class GenerateApiKeyValidator : AbstractValidator<GenerateApiKeyRequest>
{
    public GenerateApiKeyValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("API key name is required");
    }
}