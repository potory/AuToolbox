namespace AutomaticToolbox.Core.Extensions;

public static class ArrayExtensions
{
    public static T OneOf<T>(this IReadOnlyList<T> collection)
    {
        var count = collection.Count;
        var random = Random.Shared.Next(0, count);

        return collection[random];
    }
}