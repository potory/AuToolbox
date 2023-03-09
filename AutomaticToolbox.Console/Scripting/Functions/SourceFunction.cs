namespace AutomaticToolbox.Console.Scripting.Functions;

public class SourceFunction : Function
{
    private readonly FunctionContext _context;

    public SourceFunction(FunctionContext context)
    {
        _context = context;
    }
    public override object Evaluate(List<object> arguments)
    {
        return _context.Source;
    }
}