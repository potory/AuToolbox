using System.Text;
using AutomaticToolbox.Console.Extensions;
using Prompthing.Core.Templates;

namespace AutomaticToolbox.Console.Generation.Sources;

public class PromptSource : Source
{
    private readonly Dataset _dataset;
    private readonly StringBuilder _stringBuilder = new();

    public PromptSource(string filePath)
    {
        var json = File.ReadAllText(filePath);
        _dataset = new DatasetCompiler().Compile(json);
    }

    public override object GetValue()
    {
        _stringBuilder.Clear();
        _dataset.Templates.OneOf().Node.Evaluate(_stringBuilder);
        return _stringBuilder.ToString();
    }
}