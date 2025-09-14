using BotManagementSystem.Core.Entities;
using FluentValidation;

namespace BotManagementSystem.Core.Validation
{
    public class BotValidator : AbstractValidator<Bot>
    {
        public BotValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Bot name is required")
                .MaximumLength(100).WithMessage("Bot name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleForEach(x => x.Integrations)
                .SetValidator(new BotIntegrationValidator());
        }
    }

    public class BotIntegrationValidator : AbstractValidator<BotIntegration>
    {
        public BotIntegrationValidator()
        {
            RuleFor(x => x.Type).IsInEnum().WithMessage("Invalid integration type");
            
            RuleFor(x => x.Configuration)
                .Must(config => config != null && config.Count > 0)
                .When(x => x.IsEnabled)
                .WithMessage("Configuration is required when integration is enabled");
        }
    }
}
