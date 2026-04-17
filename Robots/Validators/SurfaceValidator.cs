using FluentValidation;
using Robots.Models;

namespace Robots.Validators;

public class SurfaceValidator : AbstractValidator<Surface>
{
    public SurfaceValidator(int maxCoord = 50)
    {
        RuleFor(s => s.Width)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(maxCoord)
            .WithMessage($"Surface width must be between 0 and {maxCoord}");

        RuleFor(s => s.Height)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(maxCoord)
            .WithMessage($"Surface height must be between 0 and {maxCoord}");
    }
}
