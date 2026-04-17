using FluentValidation;
using Robots.Models;

namespace Robots.Validators;

public class WorldValidator : AbstractValidator<World>
{
    public WorldValidator(int maxCommands = 100, int maxCoord = 50)
    {
        RuleFor(s => s.Surface)
            .NotNull()
            .SetValidator(new SurfaceValidator(maxCoord));

        RuleFor(s => s.Robots)
            .NotEmpty();

        RuleForEach(s => s.Robots)
            .SetValidator(new RobotValidator(maxCommands, maxCoord));

        RuleForEach(s => s.Robots)
            .Must((world, robot) => robot.X >= 0 && robot.Y >= 0
                && robot.X <= world.Surface.Width && robot.Y <= world.Surface.Height)
            .WithMessage("Robot starting position must be within the surface bounds");
    }
}
