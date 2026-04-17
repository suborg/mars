namespace Robots.Services;

public interface IAppLogger
{
    void Log(string message);
    void LogError(string message);
}
