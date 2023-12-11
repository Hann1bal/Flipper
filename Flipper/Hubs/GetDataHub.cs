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
        var datas = new { cards = await _repository.GetRange() };

        var data = JsonConvert.SerializeObject(datas);
        await Clients.All.SendAsync("GetData", data);
        base.OnConnectedAsync();
    }

    public async Task GetData()
    {
        var datas = new { cards = await _repository.GetRange() };

        var data = JsonConvert.SerializeObject(datas);
        await Clients.All.SendAsync("GetData", data);
    }

    public async Task Ping(string message)
    {
        Console.WriteLine(message);
        await Clients.All.SendAsync("Ping", "pong");
    }
}