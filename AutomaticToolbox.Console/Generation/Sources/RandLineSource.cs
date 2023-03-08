using AutomaticToolbox.Console.Extensions;

namespace AutomaticToolbox.Console.Generation.Sources;

public class RandLineSource : Source
{
    private readonly string[] _lines;

    public RandLineSource(string filePath)
    {
        _lines = File.ReadAllLines(filePath);
    }

    public override object GetValue()
    {
        return _lines.OneOf();
    }
}