using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Configurations;

public class OverrideConfig
{
    public JObject Json { get; }

    public OverrideConfig(JObject json)
    {
        Json = json;
    }
}