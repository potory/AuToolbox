using AutomaticToolbox.Console.Extensions;
using AutomaticToolbox.Console.Generation.Sources;
using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Generation.Operations;

public interface ITransformOperation
{
    object Evaluate(object obj);
}

public abstract class BasicTransformOperation : ITransformOperation
{
    public abstract object Evaluate(object obj);

    public static ITransformOperation FromJToken(JToken jToken)
    {
        var obj = ((JObject)jToken).MustBeNotNull("object must be not null");

        string operation = obj.Value<string>("operation");

        switch (operation)
        {
            case "remove":
            {
                string value = (obj.Value<string>("value")).MustBeNotNull("value must be not null");
                return new RemoveTransformOperation(value);
            }
            case "insert":
            {
                string value = (obj.Value<string>("value")).MustBeNotNull("value must be not null");
                var position = obj.Value<int>("position");
                return new InsertTransformOperation(position, value);
            }
            case "multiply":
            {
                double value = obj.Value<double>("value");
                return new MultiplyTransformOperation(value);
            }
            default:
                throw new ArgumentException($"Invalid operation string: {jToken}");
        }
    }
}

public abstract class BasicOperation<T> : BasicTransformOperation
{
    public override object Evaluate(object obj)
    {
        if (obj is not T val)
        {
            throw new ArgumentException($"The argument provided is not of type {typeof(T)}");
        }

        return Evaluate(val);
    }

    protected abstract T Evaluate(T source);
}