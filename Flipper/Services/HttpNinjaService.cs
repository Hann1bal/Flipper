using Flipper.Models;
using Flipper.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Flipper.Services;

public class HttpNinjaService
{
    private readonly ApiRequestSenderService _api;
    private readonly UpdateService _updateService;
    private readonly IDbContextFactory<FlipperContext> _dbContextFactory;

    public HttpNinjaService(ApiRequestSenderService api, UpdateService updateService,
        IDbContextFactory<FlipperContext> dbContextFactory)
    {
        _api = api;
        _updateService = updateService;
        _dbContextFactory = dbContextFactory;
    }

    public async Task StartSync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        var linesUniq = new List<Uniq>();
        var response =
            await _api.GetAsync("Affliction", "DivinationCard", "itemoverview", "{0}/{3}?league={1}&type={2}");

        var responseCurrency =
            await _api.GetAsync("Affliction", "Currency", "currencyoverview", "{0}/{3}?league={1}&type={2}");
        var responseGem = await _api.GetAsync("Affliction", "SkillGem", "itemoverview", "{0}/{3}?league={1}&type={2}");
        foreach (var type in new List<string>
                     { "UniqueAccessory", "UniqueArmour", "UniqueWeapon", "UniqueFlask", "UniqueJewel" })
        {
            var responseUniq = await _api.GetAsync("Affliction", type, "itemoverview", "{0}/{3}?league={1}&type={2}");
            linesUniq.AddRange(JsonConvert.DeserializeObject<ResponseUniqDto>(responseUniq)!.lines);
        }
        var linesGems = JsonConvert.DeserializeObject<ResponseGemDto>(responseGem)!.lines;
        var linesCurrency = JsonConvert.DeserializeObject<ResponseCurrencyDto>(responseCurrency)!.lines;

        var linesGemAdd = new List<Gem>();
        foreach (var t in linesGems)
        {
            if (!context.Gem.Any(c => c.idCards == t.idCards)) linesGemAdd.Add(t);
        }
        var linesUniqAdd = new List<Uniq>();
        foreach (var t in linesUniq)
        {
            if (!context.Uniqs.Any(c => c.idCards == t.idCards)) linesUniqAdd.Add(t);
        }
        var linesCurrencyAdd = new List<Currency>();

        foreach (var t in linesCurrency)
        {
            if (!context.Currency.Any(c => c.id == t.id)) linesCurrencyAdd.Add(t);
        }
        await context.Uniqs.AddRangeAsync(linesUniqAdd);
        await context.Gem.AddRangeAsync(linesGemAdd);
        await context.Currency.AddRangeAsync(linesCurrencyAdd);
        await context.SaveChangesAsync();
        await _updateService.UpdateCards(JsonConvert.DeserializeObject<ResponseDto>(response)!.lines);

    }
}