using System.Text;
using Robots.Models;

namespace Robots.Services;

public static class FormatLog
{
    public static string Format(World world)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var robot in world.Robots)
        {
            sb.Append($"{robot.X} {robot.Y} {robot.Orientation}");
            if (robot.IsLost)
            {
                sb.Append(" LOST");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static string MultilineMapASCIIImageOfWorld(World world)
    {
        int w = world.Surface.Width;
        int h = world.Surface.Height;
        int pad = 2; // Extra padding around the surface to see motions behind the edges

        int minX = -pad;
        int maxX = w + pad;
        int minY = -pad;
        int maxY = h + pad;

        // Build robot lookup: (x,y) -> (number, orientation)
        var robotMap = new Dictionary<(int, int), (int num, Orientation dir)>();
        for (int i = 0; i < world.Robots.Count; i++)
        {
            var r = world.Robots[i];
            robotMap[(r.X, r.Y)] = (i + 1, r.Orientation);
        }

        static char Arrow(Orientation o) => o switch
        {
            Orientation.N => '↑',
            Orientation.E => '→',
            Orientation.S => '↓',
            Orientation.W => '←',
            _ => '?'
        };

        int yLabelWidth = Math.Max(minY.ToString().Length, maxY.ToString().Length) + 1;
        int cellWidth = 4;

        StringBuilder sb = new StringBuilder();

        for (int y = maxY; y >= minY; y--)
        {
            sb.Append(y.ToString().PadLeft(yLabelWidth));
            sb.Append(' ');

            for (int x = minX; x <= maxX; x++)
            {
                bool insideX = (x >= 0 && x <= w);
                bool insideY = (y >= 0 && y <= h);
                bool isDangerous = insideX && insideY && world.DangerousCells.Contains((x, y));

                if (robotMap.TryGetValue((x, y), out var robot))
                {
                    string marker = isDangerous ? "!" : " ";
                    sb.Append($"{marker}{robot.num}{Arrow(robot.dir)} ");
                }
                else if (isDangerous)
                {
                    sb.Append("!   ");
                }
                else if (insideX && insideY)
                {
                    sb.Append(".   ");
                }
                else
                {
                    sb.Append("    ");
                }
            }

            sb.AppendLine();
        }

        sb.Append(new string(' ', yLabelWidth + 1));
        for (int x = minX; x <= maxX; x++)
        {
            sb.Append(x.ToString().PadRight(cellWidth));
        }
        sb.AppendLine();

        return sb.ToString();
    }
}
