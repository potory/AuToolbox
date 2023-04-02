using AuToolbox.Core.Configurations;

namespace AuToolbox.Core.Processing;

/// <summary>
/// A sealed class that inherits from <see cref="ImageProcessor"/> and is responsible for transforming images using the StableDiffusion API.
/// </summary>
public sealed class ImageTransformer : ImageProcessor
{
    private const string Image2ImageEndpoint = "sdapi/v1/img2img";

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTransformer"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the StableDiffusion server.</param>
    /// <param name="outputPath">The path where the transformed images will be saved.</param>
    /// <param name="iteration">The iteration index for the image transformation process.</param>
    public ImageTransformer(string ip, string outputPath, int iteration) : base(ip, outputPath, iteration)
    {
        
    }
    
    /// <inheritdoc/>
    protected override async Task<string> GetResultImage(Config request)
    {
        Stream stream = StreamConverter.RequestToStream(request, request.ImagePath);
        var resultImage = await RequestHandler.Send(Ip + Image2ImageEndpoint, stream);

        await stream.DisposeAsync();
        return resultImage;
    }
}