using Flipper.Models;
using Flipper.Repository;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Flipper.Hubs;

public class GetDataHub : Hub
{
    private readonly IBaseRepository<Cards> _repository;

    public GetDataHub(IBaseRepository<Cards> repository)
    {
        _repository = repository;
    }

    public async override Task OnConnectedAsync()

    {
        var cards = await _repository.GetRange();
        var datas = new { cards = cards.Where(c => c.profitChaos > 3).ToList() };

        var data = JsonConvert.SerializeObject(datas);
        await Clients.All.SendAsync("GetData", data);
        base.OnConnectedAsync();
    }

    public async Task GetData()
    {
        var cards = await _repository.GetRange();
        var datas = new { cards = cards.Where(c => c.profitChaos > 3).ToList() };
        var data = JsonConvert.SerializeObject(datas);
        await Clients.All.SendAsync("GetData", data);
    }

    public async Task Ping(string message)
    {
        Console.WriteLine(message);
        await Clients.All.SendAsync("Ping", "pong");
    }
}