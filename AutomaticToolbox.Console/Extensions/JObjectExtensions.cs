using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Extensions;

public static class JObjectExtensions
{
    public static void CopyExistingFields(this JObject target, JObject source)
    {
        foreach (var property in source.Properties())
        {
            if (target[property.Name] != null)
            {
                target[property.Name] = property.Value;
            }
        }
    }
}