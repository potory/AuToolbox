namespace AutomaticToolbox.Core.Scripting.Functions;

public class IntFunction : Function
{
    public override object Evaluate(List<object> arguments)
    {
        return arguments[0] switch
        {
            double d => (int) d,
            string s => int.Parse(s),
            _ => throw new ArgumentException()
        };
    }
}