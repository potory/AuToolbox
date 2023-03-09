using AutomaticToolbox.Console.Extensions;
using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Configurations;

public class GenerationConfig
{
    private const string DefaultTextToImagePath = "Defaults/DefaultTextToImageRequest.json";
    private const string DefaultImageToImagePath = "Defaults/DefaultImageToImageRequest.json";

    private readonly Config[] _defaults;
    private readonly Config[] _overrides;

    public Config DefaultTextToImage => _defaults[0];
    public Config DefaultImageToImage => _defaults[1];

    public GenerationConfig(string path)
    {
        var parentDirectory = GetParentDirectory(path);
        var configsPaths = GetConfigsPaths(path);

        _overrides = CreateOverrides(parentDirectory, configsPaths);
        _defaults = new[]
        {
            GetConfig(DefaultTextToImagePath),
            GetConfig(DefaultImageToImagePath)
        };
    }

    public Config OverridesFor(int iteration) => 
        _overrides[iteration];

    private static Config[] CreateOverrides(string parentDirectory, IReadOnlyList<string> configsPaths)
    {
        var overrides = new Config[configsPaths.Count];

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

    private static Config GetConfig(string configPath) => 
        new(JObject.Parse(File.ReadAllText(configPath)));

    private static string[] GetConfigsPaths(string path) => 
        JArray.Parse(File.ReadAllText(path)).ToArrayOf<string>();
}