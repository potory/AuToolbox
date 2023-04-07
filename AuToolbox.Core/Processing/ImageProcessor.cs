using AuToolbox.Core.Configurations;
using AuToolbox.Core.Implementations;

namespace AuToolbox.Core.Processing;

/// <summary>
/// Base class for processing images.
/// </summary>
public abstract class ImageProcessor
{
    /// <summary>
    /// Index of iteration.
    /// </summary>
    private readonly int _iteration;
    
    /// <summary>
    /// Path for saving the output images.
    /// </summary>
    private readonly string _outputPath;
    
    /// <summary>
    /// Stopwatch for measuring elapsed time.
    /// </summary>
    private readonly IteratedStopwatch _stopwatch;

    /// <summary>
    /// IP address for the image processing server.
    /// </summary>
    protected readonly string Ip;
    
    /// <summary>
    /// Request handler for single image processing.
    /// </summary>
    protected readonly SingleImageRequestHandler RequestHandler;
    
    /// <summary>
    /// Converter for stream.
    /// </summary>
    protected readonly StandardStreamConverter StreamConverter;

    /// <summary>
    /// The progress of the image processing operation.
    /// </summary>
    public double Progress { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ImageProcessor class.
    /// </summary>
    /// <param name="ip">IP address for the image processing server.</param>
    /// <param name="outputPath">Path for saving the output images.</param>
    /// <param name="iteration">Index of iteration.</param>
    protected ImageProcessor(string ip, string outputPath, int iteration)
    {
        Ip = ip;
        _outputPath = outputPath;
        _iteration = iteration;

        RequestHandler = new SingleImageRequestHandler();
        StreamConverter = new StandardStreamConverter();

        _stopwatch = new IteratedStopwatch();
    }

    /// <summary>
    /// Runs the image processing for the given configurations.
    /// </summary>
    /// <param name="configs">Array of configurations.</param>
    /// <param name="cancellationToken"></param>
    public async Task Run(Config[] configs, CancellationToken cancellationToken)
    {
        _stopwatch.Start(configs.Length);
        Progress = 0;

        for (var index = 0; index < configs.Length && !cancellationToken.IsCancellationRequested; index++)
        {
            var imageConfig = configs[index];

            var resultImage = await GetResultImage(imageConfig);
            var savePath = GetSavePath(_outputPath, _iteration, index);

            await WriteResultImageToFile(resultImage, savePath);

            imageConfig.SetImagePath(savePath);
            _stopwatch.NextIteration();

            Progress = (double) (index+1) / configs.Length;
        }
    }

    /// <summary>
    /// Gets the result image for the given configuration.
    /// </summary>
    /// <param name="request">Configuration for the image processing request.</param>
    /// <returns>Base64-encoded string representation of the result image.</returns>
    protected abstract Task<string> GetResultImage(Config request);

    /// <summary>
    /// Gets the path for saving the image.
    /// </summary>
    /// <param name="outputPath">Path for saving the output images.</param>
    /// <param name="iteration">Number of iterations.</param>
    /// <param name="index">Index of the image.</param>
    /// <returns>Path for saving the image.</returns>
    private static string GetSavePath(string outputPath, int iteration, int index)
    {
        var savePath = Path.Combine(Path.GetFullPath(outputPath), iteration.ToString(), GetImageName(index));
        var directoryName = Path.GetDirectoryName(savePath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName!);
        }

        return savePath;
    }

    /// <summary>
    /// Writes the result image to file.
    /// </summary>
    /// <param name="resultImage">Base64-encoded string representation of the result image.</param>
    /// <param name="savePath">Path for saving the image.</param>
    private static async Task WriteResultImageToFile(string resultImage, string savePath) => 
        await File.WriteAllBytesAsync(savePath, Convert.FromBase64String(resultImage));

    /// <summary>
    /// Gets the name for the image file.
    /// </summary>
    /// <param name="imageIndex">Index of the image.</param>
    /// <returns>Name for the image file.</returns>
    private static string GetImageName(int imageIndex) => 
        $"{imageIndex:0000}.png";
}