using AuToolbox.Core.Abstraction;
using Newtonsoft.Json.Linq;

namespace AuToolbox.Core.Implementations;

public class SingleImageRequestHandler : IRequestHandler<string>
{
    private readonly HttpClient _client;

    public SingleImageRequestHandler()
    {
        _client = new HttpClient();
        _client.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<string> Send(string address, Stream contentStream)
    {
        var content = new StreamContent(contentStream);
        var resultContent = _client.PostAsync(address, content).Result.Content;
        var stream = await resultContent.ReadAsStreamAsync();

        using var sr = new StreamReader(stream);
        var response = await sr.ReadToEndAsync();

        var imageContent = JObject.Parse(response)["images"]![0]!.ToString();

        return imageContent;
    }
}