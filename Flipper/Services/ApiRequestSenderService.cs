using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Flipper.Services;

public class ApiRequestSenderService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public ApiRequestSenderService(IHttpClientFactory httpClient, IConfiguration configuration)
    {
        _clientFactory = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetAsync(string league, string type, string end, string endpoint = "")
    {
        var client = _clientFactory.CreateClient();
        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri =
                new Uri(string.Format(endpoint, _configuration["PoeNinja"], league, type, end))
        };
        var httpResponse = await client.SendAsync(message).ConfigureAwait(false);
        return await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    //
    // public async Task GetFileAsync(string league, string url = "")
    // {
    //     var client = _clientFactory.CreateClient();
    //     Console.WriteLine("start");
    //     await using var stream = await client.GetStreamAsync(new Uri(string.Format(url, league)));
    //     await using var fs = new FileStream("C:\\Users\\Andrey\\RiderProjects\\Flipper\\Flipper/characters.csv", FileMode.OpenOrCreate);
    //     await stream.CopyToAsync(fs);
    //     Console.WriteLine("end");
    //
    //     Console.WriteLine("FileDownloaded");
    // }
    public async Task<string> GetFileAsync(string league, string url = "")
    {
        var client = _clientFactory.CreateClient();
        Console.WriteLine("start");
        var message = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri =
                new Uri(string.Format(url, league))
        };
        var httpResponse = await client.SendAsync(message).ConfigureAwait(false);
        Console.WriteLine("end");
        return await httpResponse.Content.ReadAsStringAsync();
    }
}