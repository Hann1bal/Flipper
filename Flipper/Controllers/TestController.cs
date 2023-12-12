﻿using Flipper.Models;
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
    private readonly IBaseRepository<AccountModel> _repository2;
    private readonly IBaseRepository<Uniq> _baseRepository;
    private readonly IBaseRepository<Currency> _currency;
    private readonly IBaseRepository<Gem> _gem;
    private readonly UpdateService _updateService;
    public TestController(ApiRequestSenderService api, IBaseRepository<Cards> repository, IBaseRepository<Uniq> baseRepository, IBaseRepository<Currency> currency, IBaseRepository<Gem> gem, UpdateService updateService, IBaseRepository<AccountModel> repository2)
    {
        _api = api;
        _repository = repository;
        _baseRepository = baseRepository;
        _currency = currency;
        _gem = gem;
        _updateService = updateService;
        _repository2 = repository2;
    }

    [HttpGet]
    [Route("TryParse")]
    public async Task<ActionResult<List<AccountModel>>> TryToParse()
    {
        var result2 = await _repository2.GetRange();
        return Ok(result2);
    }

    [HttpGet]
    [Route("TestOrder")]
    public async Task<IActionResult> TestOrder()
    {
        var result = await _repository.GetRange();
        return Ok(result.OrderByDescending(c=>c.profitChaosPerTrade).ThenByDescending(c=>c.profitChaos));

    }
}