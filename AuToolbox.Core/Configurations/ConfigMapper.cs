using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SonScript.Core;

namespace AuToolbox.Core.Configurations;

public class ConfigMapper
{
    private readonly FunctionContext _context;
    private readonly FunctionParser _parser;

    private Config _source;

    public ConfigMapper(IServiceProvider provider)
    {
        _context = provider.GetService<FunctionContext>();
        _parser = provider.GetService<FunctionParser>();
    }

    public void Map(Config config) => 
        MapObject(config.Json);

    public void SetSource(Config source) => 
        _source = source;

    private JToken MapObject(JObject obj)
    {
        var properties = obj.Properties();

        foreach (var property in properties)
        {
            property.Value = MapToken(property.Value);
        }

        return obj;
    }

    private JToken MapArray(JArray array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            array[i] = MapToken(array[i]);
        }

        return array;
    }

    private JToken MapValue(JValue value)
    {
        if (value.Type != JTokenType.String)
        {
            return value;
        }

        var source = GetSource(value);
        _context.SetSource(source);

        var str = (string) value.Value;

        if (string.IsNullOrEmpty(str) || str[0] != '#')
        {
            return value;
        }

        var func = _parser.Parse(str);
        var obj = func.Evaluate();

        return JToken.FromObject(obj);
    }

    private JToken MapToken(JToken token)
    {
        return token switch
        {
            JObject obj => MapObject(obj),
            JArray array => MapArray(array),
            JValue value => MapValue(value),
            _ => throw new ArgumentException()
        };
    }

    private static T GetValue<T>(JToken token) => 
        (T)((JValue)token).Value;

    private object GetSource(JValue value) => 
        ((JValue) _source?.Json[value.Path])?.Value;
}