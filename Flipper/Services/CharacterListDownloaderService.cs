using Flipper.Models;
using Flipper.Repository;

namespace Flipper.Services;

public class CharacterListDownloaderService
{
    private const string _baseUrl = "https://www.pathofexile.com/ladder/export-csv/league/{0}?realm=pc";
    private readonly ApiRequestSenderService _sender;
    private readonly IBaseRepository<Account> _repository;

    public CharacterListDownloaderService(ApiRequestSenderService sender, IBaseRepository<Account> repository)
    {
        _sender = sender;
        _repository = repository;
    }

    private async Task Parse(string data)
    {
        var AccountsList = new List<Account>();
        var AccountsListAdd = new List<Account>();
        var AccountsListUpdate = new List<Account>();
        var t = data.Split("\n");
        foreach (var ts in t)
        {
            var acc = new Account();
            var attrs = ts.Split(",");
            if (attrs.Length <= 1) continue;
            if (attrs.Contains("Rank")) continue;
            int.TryParse(attrs[0], out var rank);
            acc.Rank = rank;
            acc.AccountName = attrs[1];
            int.TryParse(attrs[4], out var level);
            long.TryParse(attrs[5], out var exp);

            acc.Characters = new List<Character>()
            {
                new ()
                {
                    Class = attrs[3],
                    Level = level,
                    Experiance = exp,
                    Name = attrs[2]
                }
            };
            AccountsList.Add(acc);
        }

        Console.WriteLine(t);
        foreach (var acc in await _repository.GetRange())
        {
            if (AccountsList.Contains(acc)) AccountsListUpdate.Add(acc);
            else AccountsListAdd.Add(acc);
        }

        await _repository.AddRange(AccountsListAdd);
        await _repository.UpdateRange(AccountsListUpdate);
    }

    public async Task GetCsv()
    {
        var chars = await _sender.GetFileAsync("Affliction", _baseUrl);
        await Parse(chars);
    }
}