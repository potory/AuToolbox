namespace AutomaticToolbox.Core.Scripting.Nodes;

public class ValueNode : FunctionNode
{
    public string Value { get; }

    public ValueNode(string value)
    {
        Value = value;
    }

    public override object Evaluate()
    {
        return Value;
    }
}