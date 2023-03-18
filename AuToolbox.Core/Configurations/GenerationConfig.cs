using Newtonsoft.Json.Linq;

namespace AuToolbox.Core.Configurations;

public class GenerationConfig
{
    private const string DefaultTextToImagePath = "Defaults/DefaultTextToImageRequest.json";
    private const string DefaultImageToImagePath = "Defaults/DefaultImageToImageRequest.json";

    private readonly Config[] _defaults;
    private readonly Config[] _overrides;

    private readonly string[] _iterationsNames;

    public Config DefaultTextToImage => _defaults[0];
    public Config DefaultImageToImage => _defaults[1];

    public int Iterations => _overrides.Length;
    public string[] IterationsNames => _iterationsNames;

    public GenerationConfig(string path)
    {
        var parentDirectory = GetParentDirectory(path);

        var props = GetProperties(path);
        _iterationsNames = GetIterationNames(props);
        
        var configsContent = props.Select(x => x.Value).ToArray();

        _overrides = CreateOverrides(parentDirectory, configsContent);
        _defaults = new[]
        {
            GetConfig(DefaultTextToImagePath),
            GetConfig(DefaultImageToImagePath)
        };
    }

    public Config OverridesFor(int iteration) => 
        _overrides[iteration];

    private static Config[] CreateOverrides(string parentDirectory, JToken[] configsPaths)
    {
        var overrides = new Config[configsPaths.Length];

        for (int index = 0; index < configsPaths.Length; index++)
        {
            if (configsPaths[index] is JValue value)
            {
                string fullPath = GetFullPath(parentDirectory, value.Value<string>());
                overrides[index] = GetConfig(fullPath);
            }
            else
            {
                overrides[index] = new Config((JObject)configsPaths[index]);
            }
        }

        return overrides;
    }

    private static string[] GetConfigsPaths(string path, out string[] epochNames)
    {
        var props = GetProperties(path);
        epochNames = GetIterationNames(props);
        return props.Select(x => x.Value.Value<string>()).ToArray();
    }

    private static JProperty[] GetProperties(string path) => 
        JObject.Parse(File.ReadAllText(path)).Properties().ToArray();

    private static string GetFullPath(string parentDirectory, string localPath) => 
        Path.GetFullPath(Path.Combine(parentDirectory!, localPath!));

    private static string GetParentDirectory(string path) => 
        Path.GetDirectoryName(Path.GetFullPath(path));

    private static Config GetConfig(string configPath) => 
        new(JObject.Parse(File.ReadAllText(configPath)));

    private static string[] GetIterationNames(JProperty[] properties) => 
        properties.Select(x => x.Name).ToArray();
}