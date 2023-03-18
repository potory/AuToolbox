
using Newtonsoft.Json;

namespace AuToolbox.Console.Commands.PromptTemplate;

[Serializable]
internal class CategoryJson
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("values")]
    public string[] Values { get; set; }
}