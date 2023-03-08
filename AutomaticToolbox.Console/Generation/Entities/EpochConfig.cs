namespace AutomaticToolbox.Console.Generation.Entities;

public class EpochConfig
{
    public int Epoch { get; }
    public IReadOnlyList<Transform> Values { get; }

    public EpochConfig(int epoch, IReadOnlyList<Transform> values)
    {
        Epoch = epoch;
        Values = values;
    }
}