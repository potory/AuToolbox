using System.Text.RegularExpressions;
using AutomaticToolbox.Console.Commands.PromptTemplate;
using ConsoleFramework;
using ConsoleFramework.Attributes;
using Newtonsoft.Json;
using Prompthing.Core;
using Prompthing.Core.Abstract;
using Prompthing.Core.Entities;
using Prompthing.Core.Templates;
using Prompthing.Core.Templates.Nodes;
using Prompthing.Core.Templates.Nodes.Basic;
using Prompthing.Core.Utilities;

namespace AutomaticToolbox.Console.Commands;

[Command(name: "prompt-template", description: "Generates an example Prompthing JSON dataset.")]
public partial class PromptTemplateCommand : ICommand
{
    /// <summary>
    /// Regular expression used to split the template into segments.
    /// </summary>
    [GeneratedRegex("({{[^}]+}})")]
    private static partial Regex SplitRegex();

    private readonly Regex _splitRegex = SplitRegex();
    
    [Argument(Name = "savePath", Required = true, Description = "The path where the example Prompthing JSON dataset will be saved.")]
    [ExampleValue(@"C:\Datasets\exampleDataset.json")]
    public string SavePath { get; set; }
    [Argument(Name = "template", Required = false, Description = "The template from which JSON is going to be generated.")]
    [ExampleValue("\"a {{gender}} holding {{item}}\"")]
    public string Template { get; set; }

    public void Evaluate()
    {
        // Check if the SavePath is null or empty
        if (string.IsNullOrEmpty(SavePath))
        {
            throw new ArgumentException("SavePath cannot be null or empty.");
        }

        // Get the full path of the SavePath
        string fullPath = Path.GetFullPath(SavePath);

        // Get the directory name of the full path
        string directoryName = Path.GetDirectoryName(fullPath);

        // Check if the directory of the SavePath exists, create it if it doesn't
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName!);
        }

        var template = string.IsNullOrEmpty(Template) ? 
            PredefinedTemplates.JsonDataset() :
            GenerateTemplateJson(Template);
        
        File.WriteAllText(fullPath, template);
        System.Console.WriteLine($"Example Prompthing JSON dataset has been generated and saved to '{fullPath}'.");
    }

    private string GenerateTemplateJson(string template)
    {
        var referencePool = new MockReferencePool();
        var interpreter = new TokenInterpreter(referencePool);

        var rootNode = new ContainerNode();

        var segments = _splitRegex.Split(template).Where(x => !string.IsNullOrEmpty(x)).ToArray();

        foreach (var segment in segments)
        {
            rootNode.AddChild(interpreter.Interpret(segment));
        }

        var fileJson = new FileJson
        {
            Categories = new List<CategoryJson>(),
            Templates = new List<TemplateJson>(),
            Wrappers = new List<WrapperJson>()
        };

        fileJson.Templates.Add(new TemplateJson()
        {
            Name = "sourceTemplate",
            IsSnippet = false,
            Template = template
        });

        foreach (var node in rootNode.GetChildren())
        {
            switch (node)
            {
                case CategoryNode categoryNode:
                    fileJson.Categories.Add(CreateCategoryJson(categoryNode));
                    break;
                case TemplateNode templateNode:
                    fileJson.Templates.Add(CreateTemplateJson(templateNode));
                    break;
                case WrapperNode wrapperNode:
                    fileJson.Wrappers.Add(CreateWrapperJson(wrapperNode));
                    break;
            }
        }

        return JsonConvert.SerializeObject(fileJson);
    }

    private static TemplateJson CreateTemplateJson(TemplateNode templateNode)
    {
        return new TemplateJson
        {
            Name = templateNode.Template.Name,
            IsSnippet = templateNode.Template.IsSnippet,
            Template = "template value 1"
        };
    }

    private static WrapperJson CreateWrapperJson(WrapperNode wrapperNode)
    {
        return new WrapperJson
        {
            Name = wrapperNode.Wrapper.Name,
            Content = "content",
            Wrapper = "example {{content}} wrapper"
        };
    }

    private static CategoryJson CreateCategoryJson(CategoryNode categoryNode)
    {
        var category = categoryNode.Category;

        var serialized = new CategoryJson
        {
            Name = category.Name,
            Values = category.Terms.Select(x => x.Text).ToArray()
        };
        return serialized;
    }
}
public class MockReferencePool : IReferencePool
{
    public DelayedReference<T> CreateReference<T>(string name) where T : class
    {
        if (typeof(T) == typeof(Category))
        {
            return new DelayedReference<T>(() => throw new NotImplementedException(), true,  (T) MockCategory(name));
        }

        if (typeof(T) == typeof(Template))
        {
            return new DelayedReference<T>(() => throw new NotImplementedException(), true, (T) MockTemplate(name));
        }
        
        if (typeof(T) == typeof(Wrapper))
        {
            return new DelayedReference<T>(() => throw new NotImplementedException(), true, (T) MockWrapper(name));
        }

        return new DelayedReference<T>(() => throw new NotImplementedException(), true, default);
    }

    private static object MockTemplate(string name) => 
        new Template(name, new TextNode("content"), true);

    private static object MockWrapper(string name) => 
        new Wrapper(name, new TextNode("pre-content"), new TextNode("post-content"));

    private static object MockCategory(string name)
    {
        return new Category(name, new Term[]
        {
            new("value 1", 1),
            new("value 2", 1),
        });
    }

    public void AddObject<T>(string name, T obj) where T : class
    {
        throw new NotImplementedException();
    }
}