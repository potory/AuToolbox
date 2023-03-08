namespace AutomaticToolbox.Console.Generation.Operations;

public class InsertTransformOperation : BasicOperation<string>
{
    private readonly int _position;
    private readonly string _value;

    public InsertTransformOperation(int position, string value)
    {
        _position = position;
        _value = value;
    }

    protected override string Evaluate(string source)
    {
        if (_position == -1)
        {
            return source + _value;
        }

        return source.Insert(_position, _value);
    }
}