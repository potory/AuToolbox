using AuToolbox.Core.Processing;
using SonScript.Core.Functions;

namespace AuToolbox.Core.Functions;

public sealed class PreviousResultFunction : Function
{
    public override object Evaluate(List<object> arguments) => 
        ImageGenerator.PreviousResultTag;
}