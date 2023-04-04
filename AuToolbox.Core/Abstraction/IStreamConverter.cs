using AuToolbox.Core.Configurations;

namespace AuToolbox.Core.Abstraction;

public interface IStreamConverter
{
    MemoryStream RequestToStream(Config request);
    MemoryStream RequestToStream(Config request, string imagePath, string resultTag);
}