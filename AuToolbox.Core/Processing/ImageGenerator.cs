using AuToolbox.Core.Configurations;

namespace AuToolbox.Core.Processing;

/// <summary>
/// A sealed class that inherits from <see cref="ImageProcessor"/> and is responsible for generating images from text using the StableDiffusion API.
/// </summary>
public sealed class ImageGenerator : ImageProcessor
{
    private const string TextToImageEndpoint = "sdapi/v1/txt2img";
    private const string Image2ImageEndpoint = "sdapi/v1/img2img";

    public const string PreviousResultTag = "AUIMGGGN:PREVIOUS_RESULT";
    public const string CustomUrlTag = "AUIMGGN:CUSTOM_URL";

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGenerator"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the StableDiffusion server.</param>
    /// <param name="outputPath">The path where the generated images will be saved.</param>
    /// <param name="iteration"></param>
    public ImageGenerator(string ip, string outputPath, int iteration) : base(ip, outputPath, iteration)
    {
        
    }

    /// <inheritdoc/>
    protected override async Task<string> GetResultImage(Config request)
    {
        string json = request.Json.ToString();
        bool needToInsert = json.Contains(PreviousResultTag);
        
        string customUrl = request.Json.Value<string>(CustomUrlTag);

        Stream requestStream;
        string url;

        if (needToInsert)
        {
            requestStream = StreamConverter.RequestToStream(request, request.ImagePath, PreviousResultTag);
            url = customUrl == null ? Ip + Image2ImageEndpoint : Ip + customUrl;
        }
        else
        {
            requestStream = StreamConverter.RequestToStream(request);
            url = customUrl == null ? Ip + TextToImageEndpoint : Ip + customUrl;
        }
        
        return await SendRequest(url, requestStream);
    }

    private async Task<string> SendRequest(string url, Stream requestStream)
    {
        var resultImage = await RequestHandler.Send(url, requestStream);

        await requestStream.DisposeAsync();
        return resultImage;
    }
}