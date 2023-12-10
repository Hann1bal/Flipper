using Flipper.Models;
using Flipper.Repository;
using Newtonsoft.Json;

namespace Flipper.Services;

public class HttpNinjaService
{
    private readonly ApiRequestSenderService _api;
    private readonly IBaseRepository<Cards> _repository;
    private readonly IBaseRepository<Uniq> _baseRepository;
    private readonly IBaseRepository<Currency> _currency;
    private readonly IBaseRepository<Gem> _gem;
    private readonly UpdateService _updateService;

    public HttpNinjaService(ApiRequestSenderService api, IBaseRepository<Cards> repository,
        IBaseRepository<Uniq> baseRepository, IBaseRepository<Currency> currency, IBaseRepository<Gem> gem,
        UpdateService updateService)
    {
        _api = api;
        _repository = repository;
        _baseRepository = baseRepository;
        _currency = currency;
        _gem = gem;
        _updateService = updateService;
    }

    public async Task StartSync()
    {
        var list = new List<Uniq>();
        var response =
            await _api.GetAsync("Affliction", "DivinationCard", "itemoverview", "{0}/{3}?league={1}&type={2}");
        var responseCurrency =
            await _api.GetAsync("Affliction", "Currency", "currencyoverview", "{0}/{3}?league={1}&type={2}");
        var responseGem = await _api.GetAsync("Affliction", "SkillGem", "itemoverview", "{0}/{3}?league={1}&type={2}");
        foreach (var type in new List<string>
                     { "UniqueAccessory", "UniqueArmour", "UniqueWeapon", "UniqueFlask", "UniqueJewel" })
        {
            var responseUniq = await _api.GetAsync("Affliction", type, "itemoverview", "{0}/{3}?league={1}&type={2}");
            list.AddRange(JsonConvert.DeserializeObject<ResponseUniqDto>(responseUniq)!.lines);
        }

        await _baseRepository.AddRange(list);
        await _gem.AddRange(JsonConvert.DeserializeObject<ResponseGemDto>(responseGem)!.lines);
        await _currency.AddRange(JsonConvert.DeserializeObject<ResponseCurrencyDto>(responseCurrency)!.lines);
        await _updateService.UpdateCards(JsonConvert.DeserializeObject<ResponseDto>(response)!.lines);
    }
}