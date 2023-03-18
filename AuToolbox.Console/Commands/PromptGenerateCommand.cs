using System.Text;
using AuToolbox.Core.Extensions;
using ConsoleFramework;
using ConsoleFramework.Attributes;
using Prompthing.Core.Templates;

namespace AuToolbox.Console.Commands;

[Command(name: "prompt-generate", description: "Generates text prompts")]
public class PromptGenerateCommand : ICommand
{
    private readonly DatasetCompiler _compiler;

    [Argument(Name = "datasetPath", Required = true, Description = "The path to the Prompthing JSON dataset.")]
    [ExampleValue("C:\\Datasets\\exampleDataset.json")]
    public string DatasetPath { get; set; }
    
    [Argument(Name = "count", Required = false, Description = "The number of prompts to generate. If not provided, the default value is used.")]
    public int? Count { get; set; }
    
    [Argument(Name = "savePath", Required = false, Description = "The path to the file where the generated prompts will be saved. If not provided, the prompts will not be saved.")]
    [ExampleValue("C:\\Datasets\\exampleOutput.txt")]
    public string SavePath { get; set; }

    public PromptGenerateCommand(DatasetCompiler compiler) => 
        _compiler = compiler;

    public void Evaluate()
    {
        Validate();

        StringBuilder sb = new StringBuilder();
        string json = File.ReadAllText(DatasetPath);

        var dataset = _compiler.Compile(json);
        int count = Count ?? 1;

        for (int i = 0; i < count; i++)
        {
            var template = dataset.Templates.OneOf();
            template.Node.Evaluate(sb);

            sb.Append('\n');
        }

        sb.Length -= 1;
        
        var contents = sb.ToString();

        if (!string.IsNullOrEmpty(SavePath))
        {
            File.WriteAllText(SavePath, contents);
        }
        
        System.Console.WriteLine("Generated prompts:");
        System.Console.WriteLine(contents);
    }

    private void Validate()
    {
        // Validate DatasetPath
        if (string.IsNullOrEmpty(DatasetPath))
        {
            throw new ArgumentException("DatasetPath is required.");
        }

        if (!File.Exists(DatasetPath))
        {
            throw new ArgumentException("DatasetPath does not exist.");
        }

        // Validate Count (if provided)
        if (Count is < 1)
        {
            throw new ArgumentException("Count must be a positive integer.");
        }

        // Validate SavePath (if provided)
        if (string.IsNullOrEmpty(SavePath))
        {
            return;
        }

        if (File.Exists(SavePath))
        {
            throw new ArgumentException("SavePath already exists.");
        }

        var directory = Path.GetDirectoryName(SavePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}