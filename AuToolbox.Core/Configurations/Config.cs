using Newtonsoft.Json.Linq;

namespace AuToolbox.Core.Configurations;

public class Config
{
    public JObject Json { get; private set; }
    public string ImagePath { get; private set; }

    public Config(JObject json)
    {
        Json = json;
    }

    public void SetImagePath(string imagePath) => 
        ImagePath = imagePath;

    public Config Clone() => new((JObject)Json.DeepClone());

    public void UpdateFrom(Config config) => 
        Json = config.Json;
}