using FluentAssertions;
using Robots.Models;
using Robots.Services;
using Robots.Validators;

namespace Robots.Tests;

public class ValidatorTests
{
    private readonly ScriptReader reader = new();

    private World Read(string script) => reader.Read(new StringReader(script));

    [Fact]
    public void WorldValidator_CoordinatesOver50_FailsValidation()
    {
        var world = new World
        {
            Surface = new Surface { Width = 51, Height = 3 },
            Robots = [new Robot { X = 0, Y = 0, Orientation = Orientation.N, Commands = [Command.F] }]
        };

        var result = new WorldValidator().Validate(world);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void WorldValidator_CommandsExceedMax_FailsValidation()
    {
        var commands = new CommandsList();
        for (int i = 0; i < 100; i++) commands.Add(Command.F);

        var world = new World
        {
            Surface = new Surface { Width = 5, Height = 5 },
            Robots = [new Robot { X = 0, Y = 0, Orientation = Orientation.N, Commands = commands }]
        };

        var result = new WorldValidator().Validate(world);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void WorldValidator_CustomMaxCommands_UsesProvidedValue()
    {
        var commands = new CommandsList();
        for (int i = 0; i < 10; i++) commands.Add(Command.F);

        var world = new World
        {
            Surface = new Surface { Width = 5, Height = 5 },
            Robots = [new Robot { X = 0, Y = 0, Orientation = Orientation.N, Commands = commands }]
        };

        var result = new WorldValidator(maxCommands: 10).Validate(world);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void WorldValidator_RobotOutsideSurface_FailsValidation()
    {
        var world = new World
        {
            Surface = new Surface { Width = 5, Height = 5 },
            Robots = [new Robot { X = 6, Y = 0, Orientation = Orientation.N, Commands = [Command.F] }]
        };

        var result = new WorldValidator().Validate(world);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("within the surface bounds"));
    }

    [Fact]
    public void WorldValidator_NegativeRobotCoordinates_FailsValidation()
    {
        var world = new World
        {
            Surface = new Surface { Width = 5, Height = 5 },
            Robots = [new Robot { X = -1, Y = -1, Orientation = Orientation.N, Commands = [Command.F] }]
        };

        var result = new WorldValidator().Validate(world);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void WorldValidator_ValidWorld_PassesValidation()
    {
        var world = Read("5 3\n1 1 E\nRF");

        var result = new WorldValidator().Validate(world);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SurfaceValidator_NegativeDimensions_FailsValidation()
    {
        var surface = new Surface { Width = -1, Height = 5 };

        var result = new SurfaceValidator().Validate(surface);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void RobotValidator_EmptyCommands_FailsValidation()
    {
        var robot = new Robot { X = 0, Y = 0, Orientation = Orientation.N, Commands = [] };

        var result = new RobotValidator().Validate(robot);

        result.IsValid.Should().BeFalse();
    }
}
