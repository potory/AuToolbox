using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Generation.Sources;

public class OneOfSource : Source
{
    private readonly JArray _options;

    public OneOfSource(JArray options)
    {
        _options = options;
    }

    public override object GetValue()
    {
        var index = Random.Shared.Next(0, _options.Count);
        return _options[index];
    }
}