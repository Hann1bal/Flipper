using Flipper.Models;

namespace Flipper.Services;

public class CharacterListDownloaderService
{
    private const string _baseUrl = "https://www.pathofexile.com/ladder/export-csv/league/{0}?realm=pc";
    private readonly ApiRequestSenderService _sender;
    private List<Accounts> AccountsList = new List<Accounts>();

    public CharacterListDownloaderService(ApiRequestSenderService sender)
    {
        _sender = sender;
    }

    private void Parse(string data)
    {
        var t = data.Split("\n");
        foreach (var ts in t)
        {
            var attrs = ts.Split(",");
            if(attrs.Length<=1)continue;
            var acc = new Accounts();
            if (attrs.Contains("Rank")) continue;
            int.TryParse(attrs[0], out var rank);
            acc.Rank = rank;
            Console.WriteLine(rank);
            Console.WriteLine(ts);
            acc.Account = attrs[1];
            acc.Characters = new List<string>() { attrs[2] };
            acc.Class = attrs[3];
            int.TryParse(attrs[4], out var level);
            acc.Level = level;
            long.TryParse(attrs[0], out var exp);
            acc.Experiance = exp;
            AccountsList.Add(acc);
        }
        Console.WriteLine(t);
    }

    public async Task GetCsv()
    {
        var chars = await _sender.GetFileAsync("Affliction", _baseUrl);
        Parse(chars);
    }
}