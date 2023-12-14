using Flipper.Models;
using Flipper.Repository;
using Flipper.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Flipper.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IBaseRepository<Cards> _repository;
    private readonly IBaseRepository<Account> _repository2;

    public TestController(ApiRequestSenderService api, IBaseRepository<Cards> repository,
        IBaseRepository<Account> repository2)
    {
        _repository = repository;
        _repository2 = repository2;
    }

    [HttpGet]
    [Route("TryParse")]
    public async Task<ActionResult<List<Account>>> TryToParse()
    {
        var result2 = await _repository2.GetRange();
        return Ok(result2);
    }

    [HttpGet]
    [Route("TestOrder")]
    public async Task<IActionResult> TestOrder()
    {
        var result = await _repository.GetRange();
        return Ok(result.OrderByDescending(c => c.profitChaosPerTrade).ThenByDescending(c => c.profitChaos));
    }
}