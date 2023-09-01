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
}