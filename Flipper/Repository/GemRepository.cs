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
        foreach (var item in items)
        {
            if (context.Gem.Any(c => c.idCards == item.idCards && c.name == item.name))
                context.Gem.Update(item);
            else await context.Gem.AddRangeAsync(item);
        }

        await context.SaveChangesAsync();
    }

    public async Task Update(Gem item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Gem.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task<List<Gem>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Gem.Include(c => c.explicitModifiers).ToListAsync();
    }
}