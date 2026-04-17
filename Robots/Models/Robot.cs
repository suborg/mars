namespace Robots.Models;

public class Robot
{
    public int X { get; set; }
    public int Y { get; set; }
    public Orientation Orientation { get; set; }
    public CommandsList Commands { get; set; } = new CommandsList();
    public bool IsLost { get; set; } = false;
}
