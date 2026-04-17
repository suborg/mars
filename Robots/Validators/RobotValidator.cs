using FluentValidation;
using Robots.Models;

namespace Robots.Validators;

public class RobotValidator : AbstractValidator<Robot>
{
    public RobotValidator(int maxCommands = 100, int maxCoord = 50)
    {
        RuleFor(r => r.X)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(maxCoord);

        RuleFor(r => r.Y)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(maxCoord);

        RuleFor(r => r.Commands)
            .NotEmpty()
            .Must(c => c.Count < maxCommands)
            .WithMessage($"Command string must be less than {maxCommands} characters");
    }
}
