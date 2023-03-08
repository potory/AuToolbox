using AutomaticToolbox.Console.Generation.Sources;

namespace AutomaticToolbox.Console.Generation.Entities;

public class DynamicField
{
    public string Field { get; }
    public Source Source { get; }

    public DynamicField(string field, Source source)
    {
        Field = field;
        Source = source;
    }
}