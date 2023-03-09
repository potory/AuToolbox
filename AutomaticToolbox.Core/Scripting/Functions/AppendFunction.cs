namespace AutomaticToolbox.Core.Scripting.Functions;

public class AppendFunction : Function
{
    public override object Evaluate(List<object> arguments) => 
        string.Join("", arguments.Select(x => (string)x));
}