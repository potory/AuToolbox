using Newtonsoft.Json.Linq;

namespace AuToolbox.Core.Extensions;

public static class JObjectExtensions
{
    public static void CopyFields(this JObject target, JObject source)
    {
        foreach (var property in source.Properties())
        {
            target[property.Name] = property.Value;
        }
    }
}