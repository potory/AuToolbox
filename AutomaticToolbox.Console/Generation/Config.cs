using AutomaticToolbox.Console.Extensions;
using AutomaticToolbox.Console.Generation.Entities;
using AutomaticToolbox.Console.Generation.Operations;
using AutomaticToolbox.Console.Generation.Sources;
using Newtonsoft.Json.Linq;

namespace AutomaticToolbox.Console.Generation;

public class Config
{
    public IReadOnlyList<DynamicField> Dynamics { get; }
    public IReadOnlyList<EpochConfig> Epochs { get; }
    public JObject Request { get; }

    public Config(JObject root)
    {
        var dynamicsArray = (JArray)root["dynamics"];
        
        if (dynamicsArray != null)
        {
            Dynamics = CreateDynamics(dynamicsArray);
        }

        var epochsArray = (JArray)root["epochs"];

        if (epochsArray != null)
        {
            Epochs = CreateEpochs(epochsArray);
        }

        Request = ((JObject)root["request"]).MustBeNotNull("The 'request' field is missing or null in the JSON configuration file.");
    }

    private static IReadOnlyList<DynamicField> CreateDynamics(JArray dynamicsArray)
    {
        var dynamics = new List<DynamicField>();

        foreach (var dynamicToken in dynamicsArray)
        {
            var name = dynamicToken["name"]?.ToString().MustBeNotNull("The 'name' field is missing or null in the 'dynamics' array of the JSON configuration file.");

            var source = Source.FromJToken(dynamicToken["source"]);
            dynamics.Add(new DynamicField(name, source));
        }

        return dynamics;
    }

    private static IReadOnlyList<EpochConfig> CreateEpochs(JArray epochsArray)
    {
        var epochs = new List<EpochConfig>();

        foreach (var epochToken in epochsArray)
        {
            var epochObject = ((JObject)epochToken).MustBeNotNull("An epoch object in the 'epochs' array of the JSON configuration file is missing or null.");

            var epoch = epochObject.Value<int>("epoch");
            var dynamicsArray = (JArray) epochObject.GetValue("dynamics");

            if (dynamicsArray == null)
            {
                continue;
            }

            var dynamics = CreateDynamicTransforms(dynamicsArray);

            epochs.Add(new EpochConfig(epoch, dynamics));
        }

        return epochs;
    }

    private static Transform[] CreateDynamicTransforms(JArray dynamicsArray)
    {
        var dynamics = new Transform[dynamicsArray.Count];

        for (var dynamicIndex = 0; dynamicIndex < dynamicsArray.Count; dynamicIndex++)
        {
            var dynamicToken = dynamicsArray[dynamicIndex];

            var dynamicObject = ((JObject)dynamicToken).MustBeNotNull("A dynamic object in the 'dynamics' array of the JSON configuration file is missing or null.");
            var name = dynamicObject.Value<string>("name");
            var transformTokensArray =
                ((JArray)dynamicObject.GetValue("transforms")).MustBeNotNull("The 'transforms' array is missing or null in a dynamic object of the JSON configuration file.");

            var operations = new ITransformOperation[transformTokensArray.Count];

            for (var operationIndex = 0; operationIndex < transformTokensArray.Count; operationIndex++)
            {
                var transformToken = transformTokensArray[operationIndex];
                var operation = BasicTransformOperation.FromJToken(transformToken);

                operations[operationIndex] = operation;
            }

            dynamics[dynamicIndex] = new Transform(name, operations);
        }

        return dynamics;
    }
}
