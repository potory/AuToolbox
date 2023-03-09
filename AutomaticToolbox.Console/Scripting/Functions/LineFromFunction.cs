using AutomaticToolbox.Console.Extensions;

namespace AutomaticToolbox.Console.Scripting.Functions;

public class LineFromFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        string path = (string)arguments[0];
        var lines = File.ReadAllLines(path);

        return lines.OneOf();
    }
}