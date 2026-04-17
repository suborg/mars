namespace Robots.Models;

public class World
{
    public required Surface Surface { get; init; }
    public required List<Robot> Robots { get; init; }
    public required List<(int X, int Y)> DangerousCells { get; init; }
}
