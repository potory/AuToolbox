namespace AutomaticToolbox.Console.Generation.Operations;

public class RemoveTransformOperation : BasicOperation<string>
{
    private readonly string _value;

    public RemoveTransformOperation(string value)
    {
        _value = value;
    }
    protected override string Evaluate(string source) => 
        source.Replace(_value, string.Empty);
}