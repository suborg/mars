namespace Robots.Models;

public class Surface
{
    public int Width { get; init; }
    public int Height { get; init; }
    public List<(int X, int Y)> DangerousCells { get; } = [];
}
