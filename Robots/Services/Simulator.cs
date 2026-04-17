using Robots.Models;

namespace Robots.Services;

public class Simulator
{
    private readonly World world;
    private readonly IAppLogger logger;
    public bool Verbose { get; set; } = false;

    public Simulator(World world, IAppLogger logger)
    {
        this.world = world;
        this.logger = logger;
    }

    private void Log(string message)
    {
        if (Verbose) logger.Log(message);
    }

    public void Run()
    {
        Log("Starting simulation...");
        foreach (var robot in world.Robots)
        {
            Log($"Starting commands for robot at ({robot.X}, {robot.Y}) facing {robot.Orientation}");
            Log(FormatLog.MultilineMapASCIIImageOfWorld(world));
            foreach (var command in robot.Commands)
            {
                if (robot.IsLost) break;
                Log($"Executing command: {command}");
                Execute(robot, command);
                Log(FormatLog.MultilineMapASCIIImageOfWorld(world));
            }
            Log($"Finished commands for robot at ({robot.X}, {robot.Y}) facing {robot.Orientation}. Lost status: {robot.IsLost}");
        }
        Log("Simulation completed.");

        Log(FormatLog.Format(world));
    }

    static void Rotate(Robot robot, Command command)
    {
        robot.Orientation = command switch
        {
            Command.L => robot.Orientation switch
            {
                Orientation.N => Orientation.W,
                Orientation.E => Orientation.N,
                Orientation.S => Orientation.E,
                Orientation.W => Orientation.S,
                _ => throw new InvalidOperationException("Unsupported orientation")
            },
            Command.R => robot.Orientation switch
            {
                Orientation.N => Orientation.E,
                Orientation.E => Orientation.S,
                Orientation.S => Orientation.W,
                Orientation.W => Orientation.N,
                _ => throw new InvalidOperationException("Unsupported orientation")
            },
            _ => throw new InvalidOperationException("Unsupported command for rotation")
        };
    }

    private void Move(Robot robot)
    {
        int nextX = robot.X;
        int nextY = robot.Y;

        switch (robot.Orientation)
        {
            case Orientation.N: nextY += 1; break;
            case Orientation.E: nextX += 1; break;
            case Orientation.S: nextY -= 1; break;
            case Orientation.W: nextX -= 1; break;
            default: throw new InvalidOperationException("Unsupported orientation");
        }

        if (IsOutOfBounds(nextX, nextY))
        {
            if (!world.DangerousCells[robot.X, robot.Y])
            {
                world.DangerousCells[robot.X, robot.Y] = true;
                robot.IsLost = true;
            }
        }
        else
        {
            robot.X = nextX;
            robot.Y = nextY;
        }
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x > world.Surface.Width || y < 0 || y > world.Surface.Height;
    }

    private void Execute(Robot robot, Command command)
    {
        Log($"Executing command: {command} for robot at ({robot.X}, {robot.Y}) facing {robot.Orientation}");

        switch (command)
        {
            case Command.L:
            case Command.R:
                Rotate(robot, command);
                break;
            case Command.F:
                Move(robot);
                break;
        }
    }
}
