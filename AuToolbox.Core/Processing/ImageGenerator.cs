using AuToolbox.Core.Configurations;

namespace AuToolbox.Core.Processing;

/// <summary>
/// A sealed class that inherits from <see cref="ImageProcessor"/> and is responsible for generating images from text using the StableDiffusion API.
/// </summary>
public sealed class ImageGenerator : ImageProcessor
{
    private const string TextToImageEndpoint = "sdapi/v1/txt2img";

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageGenerator"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the StableDiffusion server.</param>
    /// <param name="outputPath">The path where the generated images will be saved.</param>
    public ImageGenerator(string ip, string outputPath) : base(ip, outputPath, 0)
    {
        
    }

    /// <inheritdoc/>
    protected override async Task<string> GetResultImage(Config request)
    {
        Stream stream = StreamConverter.RequestToStream(request);
        var resultImage = await RequestHandler.Send(Ip + TextToImageEndpoint, stream);

        await stream.DisposeAsync();
        return resultImage;
    }
}