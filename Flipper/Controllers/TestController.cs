using Flipper.Models;
using Flipper.Repository;
using Flipper.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Flipper.Controllers;
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class TestController :ControllerBase
{
    private readonly ApiRequestSenderService _api;
    private readonly IBaseRepository<Cards> _repository;
    private readonly IBaseRepository<Uniq> _baseRepository;
    private readonly IBaseRepository<Currency> _currency;
    private readonly IBaseRepository<Gem> _gem;

    public TestController(ApiRequestSenderService api, IBaseRepository<Cards> repository, IBaseRepository<Uniq> baseRepository, IBaseRepository<Currency> currency, IBaseRepository<Gem> gem)
    {
        _api = api;
        _repository = repository;
        _baseRepository = baseRepository;
        _currency = currency;
        _gem = gem;
    }

    [HttpGet]
    [Route("TryParse")]
    public async Task<IActionResult> TryToParse()
    {
        foreach (var type in new List<string>{"UniqueAccessory", "UniqueArmour","UniqueWeapon","UniqueFlask", "UniqueJewel"})
        {
            var responseUniq = await _api.GetAsync("Ancestor", type,"itemoverview", "{0}/{3}?league={1}&type={2}");
            var resultuniq = JsonConvert.DeserializeObject<ResponseUniqDto>(responseUniq);
            await _baseRepository.AddRange(resultuniq.lines);
        }
        var response = await _api.GetAsync("Ancestor", "DivinationCard","itemoverview", "{0}/{3}?league={1}&type={2}");
        var responseCurrency = await _api.GetAsync("Ancestor", "Currency","currencyoverview", "{0}/{3}?league={1}&type={2}");
        var responseGem = await _api.GetAsync("Ancestor", "SkillGem","itemoverview", "{0}/{3}?league={1}&type={2}");
        var result = JsonConvert.DeserializeObject<ResponseDto>(response);
        var resultGem = JsonConvert.DeserializeObject<ResponseGemDto>(responseGem);
        await _gem.AddRange(resultGem.lines);

        var resultCurrencyDto = JsonConvert.DeserializeObject<ResponseCurrencyDto>(responseCurrency);
        await _currency.AddRange(resultCurrencyDto.lines);
        await _repository.AddRange(result.lines);
        var result2 = await _repository.GetRange();
        return Ok(result2.Where(c=>c.profitChaos>30));
    }

    [HttpGet]
    [Route("TestOrder")]
    public async Task<IActionResult> TestOrder()
    {
        var result = await _repository.GetRange();
        return Ok(result.OrderByDescending(c=>c.profitChaosPerTrade).ThenByDescending(c=>c.profitChaos));

    }
}