using AutomaticToolbox.Core.Extensions;

namespace AutomaticToolbox.Core.Scripting.Functions;

public class LineFromFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        string path = (string)arguments[0];
        var lines = File.ReadAllLines(path);

        return lines.OneOf();
    }
}