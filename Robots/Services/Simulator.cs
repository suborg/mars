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
            Log(FormatLog.ASCII(world));
            foreach (var command in robot.Commands)
            {
                if (robot.IsLost) break;
                Log($"Executing command: {command}");
                robot.Execute(command, world.Surface);
                Log(FormatLog.ASCII(world));
            }
            Log($"Finished commands for robot at ({robot.X}, {robot.Y}) facing {robot.Orientation}. Lost status: {robot.IsLost}");
        }
        Log("Simulation completed.");

        Log(FormatLog.Format(world));
    }
}
