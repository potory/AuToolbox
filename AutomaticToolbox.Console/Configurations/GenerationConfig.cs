using AutomaticToolbox.Console.Extensions;
using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Configurations;

public class GenerationConfig
{
    private readonly OverrideConfig[] _overrides;
    public GenerationConfig(string path)
    {
        var parentDirectory = GetParentDirectory(path);
        var configsPaths = GetConfigsPaths(path);

        _overrides = CreateOverrides(parentDirectory, configsPaths);
    }

    private static OverrideConfig[] CreateOverrides(string parentDirectory, IReadOnlyList<string> configsPaths)
    {
        var overrides = new OverrideConfig[configsPaths.Count];

        for (int index = 0; index < configsPaths.Count; index++)
        {
            string fullPath = GetFullPath(parentDirectory, configsPaths[index]);
            overrides[index] = GetConfig(fullPath);
        }

        return overrides;
    }

    private static string GetFullPath(string parentDirectory, string localPath) => 
        Path.GetFullPath(Path.Combine(parentDirectory!, localPath!));

    private static string GetParentDirectory(string path) => 
        Path.GetDirectoryName(Path.GetFullPath(path));

    private static OverrideConfig GetConfig(string configPath) => 
        new(JObject.Parse(File.ReadAllText(configPath)));

    private static string[] GetConfigsPaths(string path) => 
        JArray.Parse(File.ReadAllText(path)).ToArrayOf<string>();
}