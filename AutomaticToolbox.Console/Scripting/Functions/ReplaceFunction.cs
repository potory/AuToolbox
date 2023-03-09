namespace AutomaticToolbox.Console.Scripting.Functions;

public class ReplaceFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        var source = (string) arguments[0];
        var target = (string) arguments[1];
        var value = (string) arguments[2];

        return source.Replace(target, value);
    }
}