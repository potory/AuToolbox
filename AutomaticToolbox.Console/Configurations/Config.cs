using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Configurations;

public class Config
{
    public JObject Json { get; }

    public Config(JObject json)
    {
        Json = json;
    }
}