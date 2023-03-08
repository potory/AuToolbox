namespace AutomaticToolbox.Console.Extensions;

public static class ValidationExtension
{
    public static T MustBeNotNull<T>(this T obj, string message)
    {
        if (obj == null)
        {
            throw new ObjectValidationException(message);
        }

        return obj;
    }
}

public class ObjectValidationException : Exception
{
    public ObjectValidationException(string message) : base(message)
    {
        
    }
}