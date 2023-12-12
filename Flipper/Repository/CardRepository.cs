using Flipper.Models;
using Flipper.Services;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Repository;

public class CardRepository : IBaseRepository<Cards>
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public CardRepository(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Cards item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Cards.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task AddRange(List<Cards> items)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Cards.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public async Task Update(Cards item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Cards.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task<List<Cards>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Cards.OrderByDescending(c => c.profitChaos).ThenByDescending(c => c.profitDivine)
            .Include(c => c.explicitModifiers).ToListAsync();
    }

    public async Task UpdateRange(List<Cards> item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Cards.UpdateRange(item);
        await context.SaveChangesAsync();
    }

    public Task<Cards> Get(string name, string detailsId)
    {
        throw new NotImplementedException();
    }
}