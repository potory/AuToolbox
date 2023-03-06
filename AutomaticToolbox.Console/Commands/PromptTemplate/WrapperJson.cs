using Newtonsoft.Json;

namespace AutomaticToolbox.Console.Commands.PromptTemplate;

internal class WrapperJson
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("wrapper")]
    public string Wrapper { get; set; }
}