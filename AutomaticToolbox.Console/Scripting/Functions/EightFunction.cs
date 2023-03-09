namespace AutomaticToolbox.Console.Scripting.Functions;

public class EightFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        var num = GetDouble(arguments[0]);

        return num - num % 8;
    }
}