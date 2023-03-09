using AutomaticToolbox.Console.Extensions;

namespace AutomaticToolbox.Console.Scripting.Functions;

public class OneOfFunction : Function
{
    public override object Evaluate(List<object> arguments) => 
        arguments.OneOf();
}