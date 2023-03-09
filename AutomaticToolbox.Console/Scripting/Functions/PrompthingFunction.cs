using System.Text;
using AutomaticToolbox.Console.Extensions;
using Prompthing.Core.Templates;

namespace AutomaticToolbox.Console.Scripting.Functions;

public class PrompthingFunction : Function
{
    private readonly Dictionary<string, Dataset> _cache = new();
    private readonly StringBuilder _stringBuilder = new();

    public override object Evaluate(List<object> arguments)
    {
        _stringBuilder.Clear();

        if (arguments == null || arguments.Count == 0)
        {
            throw new ArgumentException("No arguments provided.");
        }

        if (!(arguments[0] is string path))
        {
            throw new ArgumentException("The first argument must be a string path.");
        }

        if (!File.Exists(path))
        {
            throw new ArgumentException($"The specified path '{path}' does not exist.");
        }

        if (!_cache.TryGetValue(path, out Dataset dataset))
        {
            dataset = new DatasetCompiler().Compile(File.ReadAllText(path));
            _cache.Add(path, dataset);
        }

        _cache[path].Templates.OneOf().Node.Evaluate(_stringBuilder);
        return _stringBuilder.ToString();
    }
}