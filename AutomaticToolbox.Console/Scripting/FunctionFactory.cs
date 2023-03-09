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
            "prompthing" => GetFunction(functionName),
            "source" => new SourceFunction(_context),
            "mult" => new MultFunction(),
            "multiply" => new MultFunction(),
            "eight" => new EightFunction(),
            "int" => new IntFunction(),
            "replace" => new ReplaceFunction(),
            "lineFrom" => new LineFromFunction(),
            "oneOf" => new LineFromFunction(),
            _ => throw new ArgumentException($"Unknown function '{functionName}'")
        };
    }

    private Function GetFunction(string functionName)
    {
        // TODO: Переделать все запросы на кэширование функций, сделать создание через внедрение зависимостей

        if (_cache.TryGetValue(functionName, out var func))
            return func;

        func = new PrompthingFunction();
        _cache.Add(functionName, func);

        return func;
    }
}