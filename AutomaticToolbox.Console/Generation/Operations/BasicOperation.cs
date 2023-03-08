using AutomaticToolbox.Console.Generation.Abstract;

namespace AutomaticToolbox.Console.Generation.Operations;

public abstract class BasicOperation<T> : ITransformOperation
{
    public object Evaluate(object obj)
    {
        if (obj is not T val)
        {
            throw new ArgumentException($"The argument provided is not of type {typeof(T)}");
        }

        return Evaluate(val);
    }

    protected abstract T Evaluate(T source);
}