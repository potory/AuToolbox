using AutomaticToolbox.Console.Scripting.Functions;

namespace AutomaticToolbox.Console.Scripting.Nodes;

public class FunctionCallNode : FunctionNode
{
    private readonly Function _function;
    private readonly List<FunctionNode> _arguments;

    public FunctionCallNode(Function function, List<FunctionNode> arguments)
    {
        _function = function;
        _arguments = arguments;
    }

    public override object Evaluate()
    {
        // Evaluate the arguments
        var evaluatedArgs = _arguments.Select(a => a.Evaluate()).ToList();

        // Call the function
        return _function.Evaluate(evaluatedArgs);
    }
}