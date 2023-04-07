using AuToolbox.Core.Configurations;
using AuToolbox.Core.Extensions;
using AuToolbox.Core.Processing;
using ConsoleFramework.Environment;

namespace AuToolbox.Console.Processes;

/// <summary>
/// Represents a process for generating a set of images, using a given <see cref="ConfigMapper"/> to map configurations and a <see cref="ConfigsSet"/> to generate a series of images with incremental changes.
/// Implements the <see cref="IContiguousProcess"/> interface.
/// </summary>
public class GenerationProcess : IContiguousProcess
{
    private readonly string _ip;
    private readonly string _output;

    private readonly Config[] _configs;
    private readonly ConfigsSet _configsSet;
    private readonly ConfigMapper _mapper;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private const string ProcessName = "Image Generation";
    private const string InitialMessage = "Preparing for image generation...";

    public string Name => ProcessName;
    public double Progress { get; private set; }
    public string Message { get; private set; } = InitialMessage;
    public ProcessStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationProcess"/> class with the specified parameters.
    /// </summary>
    /// <param name="ip">The IP address of the AUTOMATIC1111 Stable Diffusion API.</param>
    /// <param name="output">The output directory where generated images will be saved.</param>
    /// <param name="mapper">The <see cref="ConfigMapper"/> to use for mapping configurations.</param>
    /// <param name="configsSet">The <see cref="ConfigsSet"/> to use for generating a series of images with incremental changes.</param>
    /// <param name="configs">The initial configurations to use for generating the images.</param>
    public GenerationProcess(string ip, string output, ConfigMapper mapper, ConfigsSet configsSet, Config[] configs)
    {
        _ip = ip;
        _output = output;
        _configsSet = configsSet;
        _configs = configs;
        _mapper = mapper;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task RunAsync()
    {
        Status = ProcessStatus.Running;
        Message = "Generating initial images...";

        ImageProcessor generator = new ImageGenerator(_ip, _output, 0);
        await Generate(generator, 0, _cancellationTokenSource.Token);

        for (int iteration = 1; iteration < _configsSet.Iterations && !IsCancellationRequested(); iteration++)
        {
            generator = new ImageGenerator(_ip, _output, iteration);
            Message = $"Transforming images (step {iteration})...";

            await Generate(generator, iteration, _cancellationTokenSource.Token);
        }

        if (IsCancellationRequested())
        {
            Progress = 0;
            Message = "Image generation canceled.";
            Status = ProcessStatus.Failed;
            return;
        }

        Progress = 1;
        Message = "Image generation completed.";
        Status = ProcessStatus.Completed;
    }


    public void Cancel() => _cancellationTokenSource.Cancel();

    /// <summary>
    /// Generates images using the specified <see cref="ImageProcessor"/> and configuration overrides.
    /// </summary>
    /// <param name="generator">The <see cref="ImageProcessor"/> to use for generating the images.</param>
    /// <param name="iteration">The current iteration of the generation process.</param>
    /// <param name="cancellationToken"></param>
    private async Task Generate(ImageProcessor generator, int iteration, CancellationToken cancellationToken)
    {
        var step = 1 /(double) _configsSet.Iterations;
        var overrides = _configsSet.OverridesFor(iteration);

        try
        {
            foreach (var config in _configs)
            {
                MapConfig(config, overrides);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }

        var task = generator.Run(_configs, cancellationToken);

        while (!task.IsCompleted)
        {
            Progress = iteration * step + generator.Progress * step;
            await Task.Delay(500, cancellationToken);
        }

        Progress = (iteration + 1) * step;
    }

    /// <summary>
    /// Maps the overrides onto the request configuration and updates the JSON fields.
    /// </summary>
    /// <param name="request">The configuration to be mapped.</param>
    /// <param name="overrides">The overrides to be applied to the configuration.</param>
    private void MapConfig(Config request, Config overrides)
    {
        overrides = overrides.Clone();

        _mapper.SetSource(request);
        _mapper.Map(overrides);

        request.Json.CopyFields(overrides.Json);
    }

    private bool IsCancellationRequested() => _cancellationTokenSource.IsCancellationRequested;
}