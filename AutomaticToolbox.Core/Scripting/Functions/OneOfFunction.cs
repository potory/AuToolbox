using AutomaticToolbox.Core.Extensions;

namespace AutomaticToolbox.Core.Scripting.Functions;

public class OneOfFunction : Function
{
    public override object Evaluate(List<object> arguments) => 
        arguments.OneOf();
}