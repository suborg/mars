using Robots.Models;

namespace Robots.Services;

public class ScriptReader : IScriptReader
{
    public World Read(TextReader reader)
    {
        string? ReadNextLine()
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var trimmed = line.Trim();
                if (trimmed.Length > 0)
                    return trimmed;
            }
            return null;
        }

        var surfaceLine = ReadNextLine();
        if (surfaceLine == null)
            throw new FormatException("Script is empty.");

        var surface = ParseSurface(surfaceLine);

        var robots = new List<Robot>();
        while (true)
        {
            var positionLine = ReadNextLine();
            if (positionLine == null) break;

            var commandsLine = ReadNextLine();
            if (commandsLine == null)
                throw new FormatException($"Unexpected trailing line without commands: '{positionLine}'");

            var robot = ParseRobot(positionLine, commandsLine);
            robots.Add(robot);
        }

        if (robots.Count == 0)
            throw new FormatException("No robots found in script.");

        return new World
        {
            Surface = surface,
            Robots = robots,
            DangerousCells = new bool[surface.Width + 1, surface.Height + 1]
        };
    }

    private Surface ParseSurface(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2
            || !int.TryParse(parts[0], out int width)
            || !int.TryParse(parts[1], out int height))
            throw new FormatException($"Invalid surface line: '{line}'");

        return new Surface { Width = width, Height = height };
    }

    private Robot ParseRobot(string positionLine, string commandsLine)
    {
        var parts = positionLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3
            || !int.TryParse(parts[0], out int x)
            || !int.TryParse(parts[1], out int y)
            || !Enum.TryParse<Orientation>(parts[2], out var orientation))
            throw new FormatException($"Invalid robot position: '{positionLine}'");

        var commands = new CommandsList();
        foreach (var ch in commandsLine)
        {
            if (!Enum.TryParse<Command>(ch.ToString(), out var cmd))
                throw new FormatException($"Invalid command '{ch}' in: '{commandsLine}'");
            commands.Add(cmd);
        }

        return new Robot { X = x, Y = y, Orientation = orientation, Commands = commands };
    }
}
