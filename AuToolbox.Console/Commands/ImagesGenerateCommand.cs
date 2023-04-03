using System.Diagnostics;
using AuToolbox.Console.Processes;
using AuToolbox.Core.Configurations;
using AuToolbox.Core.Extensions;
using ConsoleFramework.Abstract;
using ConsoleFramework.Attributes;
using ConsoleFramework.Environment;

namespace AuToolbox.Console.Commands;

[Command("images-generate", "Generates images using Stable Diffusion AUTOMATIC1111 API")]
public class ImagesGenerateCommand : IAsyncCommand
{
    private const string DefaultIp = "http://127.0.0.1:7860/";
    private const string DefaultOutput = "Output\\Images";

    private readonly IContiguousProcessRunner _processRunner;
    private readonly ConfigMapper _mapper;

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

    public ImagesGenerateCommand(IContiguousProcessRunner processRunner, ConfigMapper mapper)
    {
        _processRunner = processRunner;
        _mapper = mapper;
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

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var configsSet = new ConfigsSet(ConfigPath);
        var imagesConfigs = CreateConfigs(Count, configsSet.DefaultTextToImage);

        var process = new GenerationProcess(ip, output, _mapper, configsSet, imagesConfigs);

        await _processRunner.RunProcessAsync(process);

        stopwatch.Stop();

        System.Console.Clear();
        System.Console.WriteLine(GetCompleteString(), configsSet.Iterations * imagesConfigs.Length, stopwatch.Elapsed.ToReadableString());
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
}