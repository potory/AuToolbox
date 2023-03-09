using AutomaticToolbox.Core;
using ConsoleFramework;
using ConsoleFramework.Attributes;

namespace AutomaticToolbox.Console.Commands;

[Command("images-generate", "Generates images using Stable Diffusion AUTOMATIC1111 API")]
public class ImagesGenerateCommand : ICommand
{
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

    public void Evaluate()
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

        var generator = new Generator(ip, output);
        generator.Run(ConfigPath, Count);
    }
}