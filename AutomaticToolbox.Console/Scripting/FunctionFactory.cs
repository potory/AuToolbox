using AutomaticToolbox.Console.Scripting.Functions;

namespace AutomaticToolbox.Console.Scripting;

public class FunctionFactory
{
    private readonly FunctionContext _context;
    private readonly Dictionary<string, Function> _cache = new();

    public FunctionFactory(FunctionContext context)
    {
        _context = context;
    }

    public Function CreateFunction(string functionName)
    {
        return functionName.ToLower() switch
        {
            "prompthing" => GetPrompthingFunction(functionName),
            "source" => new SourceFunction(_context),
            "mult" => new MultFunction(),
            "multiply" => new MultFunction(),
            "eight" => new EightFunction(),
            "int" => new IntFunction(),
            "replace" => new ReplaceFunction(),
            _ => throw new ArgumentException($"Unknown function '{functionName}'")
        };
    }

    private Function GetPrompthingFunction(string functionName)
    {
        if (_cache.TryGetValue(functionName, out var func))
            return func;

        func = new PrompthingFunction();
        _cache.Add(functionName, func);

        return func;
    }
}