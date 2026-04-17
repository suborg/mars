namespace Robots.Models;

public class World
{
    public required Surface Surface { get; init; }
    public required List<Robot> Robots { get; init; }
}
