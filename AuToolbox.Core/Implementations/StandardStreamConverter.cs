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

    public MemoryStream RequestToStream(Config request, string imagePath, string resultTag)
    {
        var stream = new MemoryStream();
        var sw = new StreamWriter(stream);
        string json = request.Json.ToString();

        int start = 0;
        int insertIndex = json.IndexOf(resultTag, StringComparison.Ordinal);

        if (insertIndex == -1)
            throw new ArgumentException();
        
        var imageString = GetImageString(imagePath);

        while (insertIndex > -1)
        {
            sw.Write(json.AsSpan(start, insertIndex - start));
            sw.Write(imageString);

            start = insertIndex + resultTag.Length;
            insertIndex = json.IndexOf(resultTag, start, StringComparison.Ordinal);
        }
        
        sw.Write(json.AsSpan(start, json.Length-start));

        sw.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static string GetImageString(string imagePath) => 
        Convert.ToBase64String(File.ReadAllBytes(imagePath));
}