using System.Globalization;

namespace AutomaticToolbox.Console.Scripting.Functions;

public abstract class Function
{
    public abstract object Evaluate(List<object> arguments);

    protected static double GetDouble(object obj)
    {
        if (obj is double result)
        {
            return result;
        }
        
        if (!double.TryParse(obj.ToString(), CultureInfo.InvariantCulture, out result))
        {
            throw new ArgumentException();
        }

        return result;
    }
}