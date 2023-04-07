using ConsoleFramework.Abstract;
using ConsoleFramework.Attributes;
using SonScript.Core;

namespace AuToolbox.Console.Commands;

[Command("clear-cache", "Clear cache of generation data")]
public class ClearCacheCommand : ICommand
{
    private readonly FunctionFactory _functionFactory;

    public ClearCacheCommand(FunctionFactory functionFactory)
    {
        _functionFactory = functionFactory;
    }

    public void Evaluate() => 
        _functionFactory.ClearCache();
}