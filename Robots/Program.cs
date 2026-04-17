using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Robots.Services;
using Robots.Validators;

namespace Robots;

class Program
{
    static int Main(string[] args)
    {
        var scriptOption = new Option<FileInfo>("--script", "-s")
        {
            Description = "Path to the script file with robot instructions",
            DefaultValueFactory = _ => new FileInfo("script.txt")
        };

        var verboseOption = new Option<bool>("--verbose", "-v")
        {
            Description = "Enable detailed step-by-step logging"
        };

        var maxCommandsOption = new Option<int>("--max-commands", "-m")
        {
            Description = "Maximum number of commands per robot (default: 100)",
            DefaultValueFactory = _ => 100
        };

        var maxCoordOption = new Option<int>("--max-coord", "-c")
        {
            Description = "Maximum allowed coordinate value for surface and robots (default: 50)",
            DefaultValueFactory = _ => 50
        };

        var rootCommand = new RootCommand("Robots Simulator — executes robot instructions on a rectangular grid")
        {
            scriptOption,
            verboseOption,
            maxCommandsOption,
            maxCoordOption
        };

        rootCommand.SetAction(parseResult =>
        {
            var scriptFile = parseResult.GetValue(scriptOption);
            var verbose = parseResult.GetValue(verboseOption);
            var maxCommands = parseResult.GetValue(maxCommandsOption);
            var maxCoord = parseResult.GetValue(maxCoordOption);
            return RunSimulation(scriptFile!, verbose, maxCommands, maxCoord);
        });

        return rootCommand.Parse(args).Invoke();
    }

    static int RunSimulation(FileInfo scriptFile, bool verbose, int maxCommands, int maxCoord)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAppLogger, ConsoleLogger>();
        services.AddSingleton<IScriptReader, ScriptReader>();
        using var provider = services.BuildServiceProvider();

        var logger = provider.GetRequiredService<IAppLogger>();
        var scriptReader = provider.GetRequiredService<IScriptReader>();

        if (!scriptFile.Exists)
        {
            logger.LogError($"Script file not found: {scriptFile.FullName}");
            return 1;
        }

        try
        {
            using var streamReader = new StreamReader(scriptFile.FullName);
            var world = scriptReader.Read(streamReader);

            var validator = new WorldValidator(maxCommands, maxCoord);
            var result = validator.Validate(world);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                    logger.LogError(error.ErrorMessage);
                return 1;
            }

            var simulator = new Simulator(world, logger) { Verbose = verbose };
            simulator.Run();
            Console.Write(FormatLog.Format(world));
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing script: {ex.Message}");
            return 1;
        }
    }
}
