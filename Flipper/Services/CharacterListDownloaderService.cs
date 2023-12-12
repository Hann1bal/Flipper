using Flipper.Models;
using Flipper.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Flipper.Services;

public class CharacterListDownloaderService
{
    private const string _baseUrl =
        "https://www.pathofexile.com/api/ladders?offset=0&limit=200&id={0}&type=league&realm=pc&class={1}-{2}&_={3}";

    private readonly ApiRequestSenderService _sender;
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public CharacterListDownloaderService(ApiRequestSenderService sender,
        IDbContextFactory<FlipperContext> contextFactory)
    {
        _sender = sender;
        _contextFactory = contextFactory;
    }

    private async Task ParseLadder()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        List<CacheModel> CacheModels = new() { };
        for (int i = 0; i < 7; i++)
        {
            for (int j = 1; j < 4; i++)
            {
                if (i == 0 && j > 1) break;
                int counter = 0;
                while (counter < 15000)
                {
                    var t = await _sender.GetFileAsync("Affliction", i, j, counter, _baseUrl);
                    CacheModels.Add(JsonConvert.DeserializeObject<CacheModel>(t));
                    counter += 200;
                    Thread.Sleep(2000);
                    foreach (var ts in CacheModels)
                    {
                        foreach (var entrie in ts.entries)
                        {
                            var isNewAcc = false;
                            var acc = context.Accounts.Include(c => c.characters).AsNoTracking()
                                .FirstOrDefault(c => c.AccountName == entrie.account.name);
                            if (acc == null)
                            {
                                isNewAcc = true;
                                acc = new Account();
                                acc.AccountName = entrie.account.name;
                            }

                            var character = new Character()
                            {
                                id = entrie.character.id,
                                Class = entrie.character.Class,
                                Level = entrie.character.Level,
                                Experiance = entrie.character.Experiance,
                                Name = entrie.character.Name
                            };

                            if (acc.characters.Any(c => c.Name == character.Name))
                            {
                                var tCharacter = acc.characters.FirstOrDefault(c => c.id == character.id);
                                tCharacter.Class = character.Class;
                                tCharacter.Experiance = character.Experiance;
                                tCharacter.Level = character.Level;
                                tCharacter.Name = character.Name;
                            }
                            else
                            {
                                acc.characters.Add(character);
                            }


                            if (isNewAcc) await context.Accounts.AddAsync(acc);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }

    public async Task GetAccountFromLadder()
    {
        await ParseLadder();
        Console.WriteLine("1");
    }
}