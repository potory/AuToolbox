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
    
    public async Task Run(string configPath, int count)
    {
        var generationConfig = new GenerationConfig(configPath);
        var configs = CreateConfigs(count, generationConfig);

        int maxIteration = generationConfig.Iterations;

        for (int iteration = 0; iteration < maxIteration; iteration++)
        {
            _stopwatch.Start(configs.Length);
            var currentOverride = generationConfig.OverridesFor(iteration);

            for (var index = 0; index < configs.Length; index++)
            {
                var overrides = currentOverride.Clone();
                var imageConfig = configs[index];

                if (iteration == 1)
                {
                    UpdateToImageToImage(generationConfig, imageConfig);
                }

                var resultImage = await GetResultImage(imageConfig, overrides);
                var savePath = Path.Combine(Path.GetFullPath(_outputPath), iteration.ToString(), GetImageName(index));

                var directoryName = Path.GetDirectoryName(savePath);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName!);
                }
                
                await File.WriteAllBytesAsync(savePath, Convert.FromBase64String(resultImage));
                imageConfig.SetImagePath(savePath);
            }
            
            _stopwatch.Stop();
        }
    }

    private async Task<string> GetResultImage(Config request, Config overrides)
    {
        MapRequest(request, overrides);

        string resultImage;
        Stream stream;

        if (request.ImagePath != null)
        {
            stream = _streamConverter.RequestToStream(request, request.ImagePath);
            resultImage = await _requestHandler.Send(_ip + Image2ImageEndpoint, stream);
        }
        else
        {
            stream = _streamConverter.RequestToStream(request);
            resultImage = await _requestHandler.Send(_ip + TextToImageEndpoint, stream);
        }

        await stream.DisposeAsync();
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