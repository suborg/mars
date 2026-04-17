namespace Robots.Models;

public class Robot
{
    public int X { get; set; }
    public int Y { get; set; }
    public Orientation Orientation { get; set; }
    public CommandsList Commands { get; set; } = new CommandsList();
    public bool IsLost { get; set; } = false;

    public void Execute(Command command, Surface surface)
    {
        switch (command)
        {
            case Command.L:
            case Command.R:
                Rotate(command);
                break;
            case Command.F:
                Move(surface);
                break;
        }
    }

    private void Rotate(Command command)
    {
        Orientation = command switch
        {
            Command.L => Orientation switch
            {
                Orientation.N => Orientation.W,
                Orientation.E => Orientation.N,
                Orientation.S => Orientation.E,
                Orientation.W => Orientation.S,
                _ => throw new InvalidOperationException("Unsupported orientation")
            },
            Command.R => Orientation switch
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

    private void Move(Surface surface)
    {
        int nextX = X;
        int nextY = Y;

        switch (Orientation)
        {
            case Orientation.N: nextY += 1; break;
            case Orientation.E: nextX += 1; break;
            case Orientation.S: nextY -= 1; break;
            case Orientation.W: nextX -= 1; break;
            default: throw new InvalidOperationException("Unsupported orientation");
        }

        bool outOfBounds = nextX < 0 || nextX > surface.Width || nextY < 0 || nextY > surface.Height;

        if (outOfBounds)
        {
            if (!surface.DangerousCells.Contains((X, Y)))
            {
                surface.DangerousCells.Add((X, Y));
                IsLost = true;
            }
        }
        else
        {
            X = nextX;
            Y = nextY;
        }
    }
}
