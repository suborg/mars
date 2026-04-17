using FluentAssertions;
using Robots.Models;
using Robots.Services;

namespace Robots.Tests;

public class FormatLogTests
{
    private readonly ScriptReader reader = new();
    private readonly IAppLogger logger = new NullLogger();

    private World Read(string script) => reader.Read(new StringReader(script));

    [Fact]
    public void Format_LostRobot_IncludesLostLabel()
    {
        var world = Read("5 3\n3 2 N\nFF");
        new Simulator(world, logger).Run();

        var output = FormatLog.Format(world);

        output.Should().Contain("LOST");
    }

    [Fact]
    public void Format_NotLostRobot_DoesNotIncludeLostLabel()
    {
        var world = Read("5 3\n1 1 E\nRFRFRFRF");
        new Simulator(world, logger).Run();

        var output = FormatLog.Format(world);

        output.Should().NotContain("LOST");
    }

    [Fact]
    public void Format_SampleOutput_MatchesExpected()
    {
        var script = "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";
        var world = Read(script);
        new Simulator(world, logger).Run();

        var output = FormatLog.Format(world).Trim();
        var lines = output.Split('\n').Select(l => l.Trim()).ToArray();

        lines[0].Should().Be("1 1 E");
        lines[1].Should().Be("3 3 N LOST");
        lines[2].Should().Be("2 3 S");
    }

    [Fact]
    public void ASCII_ContainsRobotMarker()
    {
        var world = Read("3 3\n1 1 E\nR");
        var image = FormatLog.ASCII(world);

        // Should contain robot marker with arrow
        image.Should().Contain("1");
    }
}
