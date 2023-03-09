using AutomaticToolbox.Console.Scripting.Functions;
using AutomaticToolbox.Console.Scripting.Nodes;

namespace AutomaticToolbox.Console.Scripting;

public class FunctionParser
{
    private readonly FunctionFactory _factory;

    private string _expression;
    private int _index;

    public FunctionParser(FunctionFactory factory)
    {
        _factory = factory;
    }

    public FunctionNode[] Parse(string expression)
    {
        _index = 0;
        _expression = expression;

        return Parse();
    }

    private FunctionNode[] Parse()
    {
        var nodes = new List<FunctionNode>();

        while (_index < _expression.Length)
        {
            var c = _expression[_index];

            if (char.IsWhiteSpace(c))
            {
                _index++;
            }
            else if (char.IsDigit(c))
            {
                nodes.Add(ParseNumber());
            }
            else if (c == '\'')
            {
                nodes.Add(ParseString());
            }
            else if (c == '#')
            {
                nodes.Add(ParseFunctionCall());
            }
            else if (c == ',')
            {
                _index++;
            }
            else if (c == ')')
            {
                break;
            }
            else
            {
                throw new ArgumentException($"Unexpected character '{c}' at index {_index}");
            }
        }

        return nodes.ToArray();
    }

    private FunctionNode ParseNumber()
    {
        var startIndex = _index;

        while (_index < _expression.Length && (char.IsDigit(_expression[_index]) || _expression[_index] == '.'))
        {
            _index++;
        }

        var value = _expression.Substring(startIndex, _index - startIndex);
        return new ValueNode(value);
    }

    private FunctionNode ParseString()
    {
        var startIndex = _index;
        _index++;

        while (_index < _expression.Length && _expression[_index] != '\'')
        {
            _index++;
        }

        if (_index == _expression.Length)
        {
            throw new ArgumentException("Unterminated string literal");
        }

        var value = _expression.Substring(startIndex + 1, _index - startIndex - 1);
        _index++;

        return new ValueNode(value);
    }

    private FunctionNode ParseFunctionCall()
    {
        var functionName = ParseFunctionName();
        var arguments = ParseArguments();

        // Create an instance of the function based on the function name
        Function function = _factory.CreateFunction(functionName);

        return new FunctionCallNode(function, arguments);
    }

    private string ParseFunctionName()
    {
        var startIndex = _index;
        _index++;

        while (_index < _expression.Length && char.IsLetterOrDigit(_expression[_index]))
        {
            _index++;
        }

        var functionName = _expression.Substring(startIndex + 1, _index - startIndex - 1);

        if (string.IsNullOrEmpty(functionName))
        {
            throw new ArgumentException("Function name is missing");
        }

        return functionName;
    }

    private List<FunctionNode> ParseArguments()
    {
        var arguments = new List<FunctionNode>();

        _index++; // skip opening parenthesis

        while (_index < _expression.Length)
        {
            var c = _expression[_index];

            if (char.IsWhiteSpace(c))
            {
                _index++;
            }
            else if (c == ')')
            {
                _index++;
                break;
            }
            else
            {
                arguments.AddRange(Parse());
            }
        }

        return arguments;
    }
}