using FluentAssertions;
using Robots.Models;
using Robots.Services;

namespace Robots.Tests;

public class ScriptReaderTests
{
    private readonly ScriptReader reader = new();

    private World Read(string script) => reader.Read(new StringReader(script));

    [Fact]
    public void Read_ValidScript_ParsesSurface()
    {
        var world = Read("5 3\n1 1 E\nRF");

        world.Surface.Width.Should().Be(5);
        world.Surface.Height.Should().Be(3);
    }

    [Fact]
    public void Read_ValidScript_ParsesRobot()
    {
        var world = Read("5 3\n1 1 E\nRF");

        world.Robots.Should().HaveCount(1);
        world.Robots[0].Should().BeEquivalentTo(new
        {
            X = 1, Y = 1,
            Orientation = Orientation.E,
            IsLost = false
        });
        world.Robots[0].Commands.Should().BeEquivalentTo(
            new[] { Command.R, Command.F });
    }

    [Fact]
    public void Read_MultipleRobots_ParsesAll()
    {
        var world = Read("5 3\n1 1 E\nRF\n3 2 N\nFF");

        world.Robots.Should().HaveCount(2);
    }

    [Fact]
    public void Read_EmptyScript_ThrowsFormatException()
    {
        var act = () => Read("");

        act.Should().Throw<FormatException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Read_InvalidSurface_ThrowsFormatException()
    {
        var act = () => Read("abc\n1 1 E\nRF");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Read_WindowsLineEndings_ParsesCorrectly()
    {
        var world = Read("5 3\r\n1 1 E\r\nRF");

        world.Surface.Width.Should().Be(5);
        world.Robots.Should().HaveCount(1);
    }

    [Fact]
    public void Read_OddTrailingLine_ThrowsFormatException()
    {
        var act = () => Read("5 3\n1 1 E\nRF\n2 2 N");

        act.Should().Throw<FormatException>()
            .WithMessage("*trailing*");
    }

    [Fact]
    public void Read_NegativeCoordinates_ParsesValues()
    {
        var act = () => Read("5 3\n-1 -1 E\nRF");

        // ScriptReader parses negative coords; validation catches them
        act.Should().NotThrow();
    }

    [Fact]
    public void Read_TrailingWhitespace_ParsesCorrectly()
    {
        var world = Read("5 3  \n  1 1 E  \n  RF  ");

        world.Surface.Width.Should().Be(5);
        world.Robots.Should().HaveCount(1);
    }
}
