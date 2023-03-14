using System.Diagnostics;
using AutomaticToolbox.Core.Configurations;
using AutomaticToolbox.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Core;

public class Generator
{
    private const string Image2ImageEndpoint = "sdapi/v1/img2img";
    private const string TextToImageEndpoint = "sdapi/v1/txt2img";

    private readonly string _ip;
    private readonly string _outputPath;

    private readonly HttpClient _client;

    public Generator(string ip, string outputPath)
    {
        _ip = ip;
        _outputPath = outputPath;
        _client = new HttpClient();
        _client.Timeout = TimeSpan.FromMinutes(5);
    }
    
    public void Run(string configPath, int count)
    {
        Stopwatch sw = new Stopwatch();
        
        var config = new GenerationConfig(configPath);
        var configs = CreateConfigs(count, config);

        int maxIteration = config.Iterations;

        for (int iteration = 0; iteration < maxIteration; iteration++)
        {
            double averageTime = -1;
            Console.WriteLine($"Starting iteration {config.IterationsNames[iteration]}");

            for (var imageIndex = 0; imageIndex < configs.Length; imageIndex++)
            {
                sw.Restart();
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
                sw.Stop();

                if (imageIndex <= 0)
                {
                    continue;
                }

                double previousMid = averageTime;

                averageTime += sw.Elapsed.Seconds;

                if (previousMid > 0)
                    averageTime /= 2;

                Console.WriteLine(
                    $"Expected remaining time for this epoch: {TimeSpan.FromSeconds(averageTime * (configs.Length - imageIndex)).Minutes} minutes");
            }
        }
    }


    private string GetResultImage(Config request, Config overrides)
    {
        MapRequest(request, overrides);

        string resultImage;
        Stream stream;

        if (request.ImagePath != null)
        {
            stream = RequestToStream(request, request.ImagePath);
            resultImage = SendRequest(_ip + Image2ImageEndpoint, stream).Result;
        }
        else
        {
            stream = RequestToStream(request);
            resultImage = SendRequest(_ip + TextToImageEndpoint, stream).Result;
        }

        stream.Dispose();
        return resultImage;
    }

    private static MemoryStream RequestToStream(Config request, string imagePath)
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        string json = request.Json.ToString();

        sw.Write(json[..^3]);
        sw.Write(",\r\n  \"init_images\": [\"");
        sw.Write(GetImageString(imagePath));
        sw.Write("\"]\r\n}");
        sw.Flush();

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private static MemoryStream RequestToStream(Config request)
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        string json = request.Json.ToString();

        sw.Write(json);
        sw.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private static void MapRequest(Config request, Config overrides)
    {
        var mapper = new ConfigMapper();
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

    private async Task<string> SendRequest(string address, Stream contentStream)
    {
        var content = new StreamContent(contentStream);
        var resultContent = _client.PostAsync(address, content).Result.Content;
        var stream = await resultContent.ReadAsStreamAsync();

        using var sr = new StreamReader(stream);
        var response = await sr.ReadToEndAsync();

        var imageContent = JObject.Parse(response)["images"]![0]!.ToString();

        return imageContent;
    }

    private static string GetImageString(string imagePath) => 
        Convert.ToBase64String(File.ReadAllBytes(imagePath));

    private static string GetImageName(int imageIndex) => 
        $"{imageIndex+1:0000}.png";
}