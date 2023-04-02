using AuToolbox.Core.Abstraction;
using AuToolbox.Core.Configurations;

namespace AuToolbox.Core.Implementations;

public class StandardStreamConverter : IStreamConverter
{
    public MemoryStream RequestToStream(Config request)
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        string json = request.Json.ToString();

        sw.Write(json);
        sw.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public MemoryStream RequestToStream(Config request, string imagePath)
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        string json = request.Json.ToString();

        sw.Write(json[..^3]);
        sw.Write(",\r\n  \"init_images\": [\"");
        sw.Write(GetImageString(imagePath));
        sw.Write("\"]\r\n}");
        sw.Flush();

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private static string GetImageString(string imagePath) => 
        Convert.ToBase64String(File.ReadAllBytes(imagePath));
}