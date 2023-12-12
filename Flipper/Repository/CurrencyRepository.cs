using Flipper.Models;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Repository;

public class CurrencyRepository:IBaseRepository<Currency>
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public CurrencyRepository(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Currency item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        await context.Currency.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task AddRange(List<Currency> items)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        foreach (var item in items)
        {
            if (context.Currency.Any(c => c.detailsId == item.detailsId && c.name == item.name))
                context.Currency.Update(item);
            else await context.Currency.AddAsync(item);
        }

        await context.SaveChangesAsync();
    }

    public Task Upsert(List<Currency> item)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Currency item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Currency.Update(item);
        await context.SaveChangesAsync();
    }

    public Task UpdateRange(List<Currency> item)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Currency>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Currency.ToListAsync();
    }
    public async Task<Currency> Get (string name, string detailsId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Currency.FirstOrDefaultAsync(c => c.name == name  || c.detailsId == detailsId );
    }
}