namespace AutomaticToolbox.Console.Generation.Operations;

public class MultiplyTransformOperation : BasicOperation<double>
{
    private readonly double _value;

    public MultiplyTransformOperation(double value)
    {
        _value = value;
    }

    protected override double Evaluate(double source)
    {
        return source * _value;
    }
}