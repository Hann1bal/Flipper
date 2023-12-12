using Flipper.Models;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Repository;

public class GemRepository : IBaseRepository<Gem>
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public GemRepository(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Gem item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        await context.Gem.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task AddRange(List<Gem> items)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var gemList = await context.Gem.ToListAsync();
        foreach (var item in items)
        {
            var gem = gemList.FirstOrDefault(c => c.idCards == item.idCards);
            if (gem != null)
            {
                gem.explicitModifiers = item.explicitModifiers;
                gem.chaosValue = item.chaosValue;
                gem.divineValue = item.divineValue;
                gem.count = item.count;
                gem.icon = item.icon;
                gem.baseType = item.baseType;
                gem.idCards = item.idCards;
                context.Gem.Update(gem);
            }
            else await context.Gem.AddAsync(item);
        }
        await context.SaveChangesAsync();
    }

    public async Task Update(Gem item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Gem.Update(item);
        await context.SaveChangesAsync();
    }

    public Task UpdateRange(List<Gem> item)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Gem>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Gem.Include(c => c.explicitModifiers).ToListAsync();
    }

    public Task<Gem> Get(string name, string detailsId)
    {
        throw new NotImplementedException();
    }
}