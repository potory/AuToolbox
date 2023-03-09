namespace AutomaticToolbox.Console.Scripting.Functions;

public class ReplaceFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        var source = (string) arguments[0];

        for (int i = 1; i < arguments.Count; i += 2)
        {
            var target = (string) arguments[i];
            var value = (string) arguments[i+1];
            source = source.Replace(target, value);
        }

        return source;
    }
}