using Newtonsoft.Json;

namespace AutomaticToolbox.Console.Commands.PromptTemplate;

[Serializable]
internal class TemplateJson
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("isSnippet")]
    public bool IsSnippet { get; set; }
    [JsonProperty("template")]
    public string Template { get; set; }
}