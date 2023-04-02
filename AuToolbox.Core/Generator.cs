using System.Diagnostics;
using AuToolbox.Core.Abstraction;
using AuToolbox.Core.Extensions;
using AuToolbox.Core.Configurations;
using AuToolbox.Core.Implementations;

namespace AuToolbox.Core;

public class Generator
{
    private const string Image2ImageEndpoint = "sdapi/v1/img2img";
    private const string TextToImageEndpoint = "sdapi/v1/txt2img";

    private readonly string _ip;
    private readonly string _outputPath;
    
    private readonly IServiceProvider _provider;
    private readonly IRequestHandler<string> _requestHandler;
    private readonly IStreamConverter _streamConverter;

    private readonly IteratedStopwatch _stopwatch;

    public Generator(IServiceProvider provider, string ip, string outputPath)
    {
        _provider = provider;
        _ip = ip;
        _outputPath = outputPath;
        
        _requestHandler = new SingleImageRequestHandler();
        _streamConverter = new StandardStreamConverter();

        _stopwatch = new IteratedStopwatch();
    }
    
    public void Run(string configPath, int count)
    {
        var config = new GenerationConfig(configPath);
        var configs = CreateConfigs(count, config);

        int maxIteration = config.Iterations;

        for (int iteration = 0; iteration < maxIteration; iteration++)
        {
            Console.WriteLine($"Starting iteration {config.IterationsNames[iteration]}");

            _stopwatch.Start(configs.Length);

            for (var imageIndex = 0; imageIndex < configs.Length; imageIndex++)
            {
                Console.WriteLine($"Processing image {imageIndex+1} for iteration {config.IterationsNames[iteration]}");

                var request = configs[imageIndex];
                var overrides = config.OverridesFor(iteration).Clone();

                if (iteration == 1)
                {
                    UpdateToImageToImage(config, request);
                }

                var resultImage = GetResultImage(request, overrides);
                var savePath = Path.Combine(Path.GetFullPath(_outputPath), config.IterationsNames[iteration], GetImageName(imageIndex));

                var directoryName = Path.GetDirectoryName(savePath);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName!);
                }

                Console.WriteLine($"Saving image {imageIndex} for iteration {iteration} to {savePath}");
                File.WriteAllBytes(savePath, Convert.FromBase64String(resultImage));
                request.SetImagePath(savePath);
                
                Console.WriteLine($"Expected remaining time for this epoch: {_stopwatch.RemainingTime.Minutes} minutes");
            }
            
            _stopwatch.Stop();
        }
    }

    private string GetResultImage(Config request, Config overrides)
    {
        MapRequest(request, overrides);

        string resultImage;
        Stream stream;

        if (request.ImagePath != null)
        {
            stream = _streamConverter.RequestToStream(request, request.ImagePath);
            resultImage = _requestHandler.Send(_ip + Image2ImageEndpoint, stream).Result;
        }
        else
        {
            stream = _streamConverter.RequestToStream(request);
            resultImage = _requestHandler.Send(_ip + TextToImageEndpoint, stream).Result;
        }

        stream.Dispose();
        return resultImage;
    }

    private void MapRequest(Config request, Config overrides)
    {
        var mapper = new ConfigMapper(_provider);
        mapper.SetSource(request);
        mapper.Map(overrides);

        request.Json.CopyExistingFields(overrides.Json);
    }

    private static Config[] CreateConfigs(int count, GenerationConfig config)
    {
        var configs = new Config[count];

        for (var index = 0; index < configs.Length; index++)
        {
            configs[index] = config.DefaultTextToImage.Clone();
        }

        return configs;
    }

    private static void UpdateToImageToImage(GenerationConfig config, Config request)
    {
        var defaultImageToImage = config.DefaultImageToImage.Clone();
        defaultImageToImage.Json.CopyExistingFields(request.Json);
        request.UpdateFrom(defaultImageToImage);
    }

    private static string GetImageName(int imageIndex) => 
        $"{imageIndex+1:0000}.png";
}