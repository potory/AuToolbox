using Newtonsoft.Json;

namespace AutomaticToolbox.Console.Commands.PromptTemplate;

[Serializable]
internal class FileJson
{
    [JsonProperty("templates")]
    public List<TemplateJson> Templates { get; set; }

    [JsonProperty("wrappers")]
    public List<WrapperJson> Wrappers { get; set; }

    [JsonProperty("categories")]
    public List<CategoryJson> Categories { get; set; }
}