using Application.Features.Agents.Commands;
using FluentValidation;

namespace Application.Features.Agents.Validators;

public class CreateAgentValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name must not be empty")
            .MaximumLength(200).WithMessage("Name must have maximum of 200 characters");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email must be valid")
            .MaximumLength(450).WithMessage("Email must have maximum of 450 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password must not be empty")
            .MinimumLength(6).WithMessage("Password must have at least 6 characters");
    }
}