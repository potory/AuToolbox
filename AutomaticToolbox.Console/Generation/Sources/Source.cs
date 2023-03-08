using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Generation.Sources;

public abstract class Source
{
    public abstract object GetValue();
    
    public static Source FromJToken(JToken jToken)
    {
        string sourceString = jToken.Value<string>("type");

        return sourceString switch
        {
            "prompthing" => new PromptSource(jToken.Value<string>("datasetPath")),
            "oneof" => new OneOfSource((JArray)jToken["value"]),
            "randline" => new RandLineSource(jToken.Value<string>("datasetPath")),
            _ => throw new ArgumentException($"Invalid source string: {sourceString}")
        };
    }
}