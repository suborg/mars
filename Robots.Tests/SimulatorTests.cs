using FluentAssertions;
using Robots.Models;
using Robots.Services;

namespace Robots.Tests;

public class SimulatorTests
{
    private readonly ScriptReader reader = new();
    private readonly IAppLogger logger = new NullLogger();

    private World Read(string script) => reader.Read(new StringReader(script));

    [Fact]
    public void Run_RobotGoesInCircle_ReturnsToStart()
    {
        var world = Read("5 3\n1 1 E\nRFRFRFRF");
        new Simulator(world, logger).Run();

        world.Robots[0].Should().BeEquivalentTo(new
        {
            X = 1, Y = 1,
            Orientation = Orientation.E,
            IsLost = false
        });
    }

    [Fact]
    public void Run_RobotFallsOff_MarkedAsLost()
    {
        var world = Read("5 3\n3 2 N\nFRRFLLFFRRFLL");
        new Simulator(world, logger).Run();

        world.Robots[0].X.Should().Be(3);
        world.Robots[0].Y.Should().Be(3);
        world.Robots[0].Orientation.Should().Be(Orientation.N);
        world.Robots[0].IsLost.Should().BeTrue();
    }

    [Fact]
    public void Run_DangerousCell_PreventsSecondFall()
    {
        var world = Read("5 3\n3 2 N\nFF\n3 2 N\nFF");
        new Simulator(world, logger).Run();

        world.Robots[0].IsLost.Should().BeTrue();
        world.Robots[1].IsLost.Should().BeFalse();
        world.Robots[1].X.Should().Be(3);
        world.Robots[1].Y.Should().Be(3);
    }

    [Fact]
    public void Run_SampleInput_MatchesSampleOutput()
    {
        var script = "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";
        var world = Read(script);
        new Simulator(world, logger).Run();

        // Robot 1: 1 1 E
        world.Robots[0].Should().BeEquivalentTo(new
            { X = 1, Y = 1, Orientation = Orientation.E, IsLost = false });

        // Robot 2: 3 3 N LOST
        world.Robots[1].Should().BeEquivalentTo(new
            { X = 3, Y = 3, Orientation = Orientation.N, IsLost = true });

        // Robot 3: 2 3 S
        world.Robots[2].Should().BeEquivalentTo(new
            { X = 2, Y = 3, Orientation = Orientation.S, IsLost = false });
    }

    [Fact]
    public void Run_RobotAtOriginGoingSouth_MarkedAsLost()
    {
        var world = Read("5 5\n0 0 S\nF");
        new Simulator(world, logger).Run();

        world.Robots[0].IsLost.Should().BeTrue();
        world.Robots[0].X.Should().Be(0);
        world.Robots[0].Y.Should().Be(0);
    }

    [Fact]
    public void Run_RobotAtOriginGoingWest_MarkedAsLost()
    {
        var world = Read("5 5\n0 0 W\nF");
        new Simulator(world, logger).Run();

        world.Robots[0].IsLost.Should().BeTrue();
        world.Robots[0].X.Should().Be(0);
        world.Robots[0].Y.Should().Be(0);
    }

    [Fact]
    public void Run_ScentOnAllFourEdges_PreventsSubsequentFalls()
    {
        // Robot 1 falls off north edge, Robot 2 falls off east, Robot 3 south, Robot 4 west
        // Then Robots 5-8 are saved by scent
        var script = "3 3\n" +
            "1 3 N\nF\n" +  // falls north
            "3 1 E\nF\n" +  // falls east
            "1 0 S\nF\n" +  // falls south
            "0 1 W\nF\n" +  // falls west
            "1 3 N\nF\n" +  // saved by scent
            "3 1 E\nF\n" +  // saved by scent
            "1 0 S\nF\n" +  // saved by scent
            "0 1 W\nF";     // saved by scent
        var world = Read(script);
        new Simulator(world, logger).Run();

        // First 4 robots should be lost
        for (int i = 0; i < 4; i++)
            world.Robots[i].IsLost.Should().BeTrue($"robot {i + 1} should be lost");

        // Last 4 robots should be saved by scent
        for (int i = 4; i < 8; i++)
            world.Robots[i].IsLost.Should().BeFalse($"robot {i + 5} should be saved by scent");
    }
}
