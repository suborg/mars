using Robots.Services;

namespace Robots.Tests;

public class NullLogger : IAppLogger
{
    public void Log(string message) { }
    public void LogError(string message) { }
}
