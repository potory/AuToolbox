using Newtonsoft.Json.Linq;

namespace AuToolbox.Core.Configurations;

public class ConfigsSet
{
    private const string DefaultRequestPath = "Defaults/DefaultRequest.json";

    private readonly Config[] _overrides;

    public Config Default { get; }

    public int Iterations => _overrides.Length;

    public ConfigsSet(string path)
    {
        var parentDirectory = GetParentDirectory(path);
        var overridesArray = GetOverridesArray(path);

        _overrides = CreateOverrides(parentDirectory, overridesArray);
        Default = GetConfig(DefaultRequestPath);
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

    private static JToken[] GetOverridesArray(string path) => 
        JArray.Parse(File.ReadAllText(path)).ToArray();

    private static string GetFullPath(string parentDirectory, string localPath) => 
        Path.GetFullPath(Path.Combine(parentDirectory!, localPath!));

    private static string GetParentDirectory(string path) => 
        Path.GetDirectoryName(Path.GetFullPath(path));

    private static Config GetConfig(string configPath) => 
        new(JObject.Parse(File.ReadAllText(configPath)));
}