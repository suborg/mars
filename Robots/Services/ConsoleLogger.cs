namespace Robots.Services;

public class ConsoleLogger : IAppLogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }

    public void LogError(string message)
    {
        Console.Error.WriteLine(message);
    }
}
