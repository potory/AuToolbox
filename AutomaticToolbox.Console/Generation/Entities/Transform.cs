using AutomaticToolbox.Console.Generation.Operations;

namespace AutomaticToolbox.Console.Generation.Entities;

public class Transform
{
    public string Name { get; }
    public IReadOnlyList<ITransformOperation> Transforms { get; }

    public Transform(string name, IReadOnlyList<ITransformOperation> transforms)
    {
        Name = name;
        Transforms = transforms;
    }
}