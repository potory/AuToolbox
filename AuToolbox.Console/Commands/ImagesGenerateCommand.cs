using System.Diagnostics;
using AuToolbox.Core;
using AuToolbox.Core.Configurations;
using AuToolbox.Core.Extensions;
using AuToolbox.Core.Processing;
using ConsoleFramework.Abstract;
using ConsoleFramework.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace AuToolbox.Console.Commands;

[Command("images-generate", "Generates images using Stable Diffusion AUTOMATIC1111 API")]
public class ImagesGenerateCommand : IAsyncCommand
{
    private readonly IServiceProvider _provider;
    private const string DefaultIp = "http://127.0.0.1:7860/";
    private const string DefaultOutput = "Output\\Images";

    [Argument(Name = "config-path", Description = "The path to the configuration file for image generation", Required = true)]
    [ExampleValue("\"C:\\Datasets\\Configs\\exampleConfig.json\"")]
    public string ConfigPath { get; set; }

    [Argument(Name = "count", Description = "The number of images to be generated", Required = true)]
    public int Count { get; set; }
    
    [Argument(Name = "ip", Description = "The IP address of the Stable Diffusion AUTOMATIC1111 API. If not provided, a default IP address will be used.", Required = false)]
    [ExampleValue("\"http://127.0.0.1:7860/\"")]
    public string Ip { get; set; }
    
    [Argument(Name = "output", Description = "The output directory for generated images. If not provided, the default output directory will be used.", Required = false)]
    [ExampleValue("\"Output\\Images\"")]
    public string Output { get; set; }

    private readonly ConfigMapper _mapper;

    public ImagesGenerateCommand(IServiceProvider provider)
    {
        _provider = provider;
        _mapper = new ConfigMapper(provider);
    }

    public async Task EvaluateAsync()
    {
        var ip = !string.IsNullOrEmpty(Ip) ? Ip : DefaultIp;
        var output = !string.IsNullOrEmpty(Output) ? Output : DefaultOutput;
    
        if (string.IsNullOrEmpty(ConfigPath))
        {
            throw new ArgumentException("Configuration file path must be provided.");
        }
        
        if (!File.Exists(ConfigPath))
        {
            throw new ArgumentException("Configuration file not found at the specified path.");
        }
    
        if (Count <= 0)
        {
            throw new ArgumentException("Count must be a positive integer.");
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();

        var configsSet = new ConfigsSet(ConfigPath);
        var imagesConfigs = CreateConfigs(Count, configsSet.DefaultTextToImage);
        
        await GenerateImages(ip, output, configsSet, imagesConfigs);

        sw.Stop();

        System.Console.Clear();
        System.Console.WriteLine(GetCompleteString(), configsSet.Iterations * imagesConfigs.Length, sw.Elapsed.ToReadableString());
    }

    private async Task GenerateImages(string ip, string output, ConfigsSet configsSet, Config[] configs)
    {
        ImageProcessor generator = ActivatorUtilities.CreateInstance<ImageGenerator>(_provider, ip, output);

        var overrides = configsSet.OverridesFor(0);

        foreach (var config in configs)
        {
            MapConfig(config, overrides);
        }

        await generator.Run(configs);

        for (int iteration = 1; iteration < configsSet.Iterations; iteration++)
        {
            generator = ActivatorUtilities.CreateInstance<ImageTransformer>(_provider, ip, output, iteration);
            overrides = configsSet.OverridesFor(iteration);

            foreach (var config in configs)
            {
                MapConfig(config, overrides);
            }

            await generator.Run(configs);
        }
    }

    private static string GetCompleteString() => 
        "Generation task completed successfully.\n\nTotal images generated: {0}\nTotal time taken: {1}";

    private static Config[] CreateConfigs(int count, Config defaultTextToImage)
    {
        var configs = new Config[count];

        for (var index = 0; index < configs.Length; index++)
        {
            configs[index] = defaultTextToImage.Clone();
        }

        return configs;
    }
    
    private void MapConfig(Config request, Config overrides)
    {
        _mapper.SetSource(request);
        _mapper.Map(overrides);

        request.Json.CopyFields(overrides.Json);
    }
}