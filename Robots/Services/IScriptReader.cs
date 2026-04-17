using Robots.Models;

namespace Robots.Services;

public interface IScriptReader
{
    World Read(TextReader reader);
}
