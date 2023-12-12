using Flipper.Models;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Repository;

public class UniqRepository : IBaseRepository<Uniq>
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public UniqRepository(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Uniq item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        await context.Uniqs.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task AddRange(List<Uniq> items)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var uniqList = await context.Uniqs.ToListAsync();
        foreach (var item in items)
        {
            var uniq = uniqList.FirstOrDefault(c => c.idCards == item.idCards && (c.links == item.links));
            if (uniq != null)
            {
                uniq.links = item.links;
                uniq.explicitModifiers = item.explicitModifiers;
                uniq.chaosValue = item.chaosValue;
                uniq.divineValue = item.divineValue;
                uniq.count = item.count;
                uniq.icon = item.icon;
                uniq.baseType = item.baseType;
                uniq.idCards = item.idCards;
                context.Uniqs.Update(uniq);
            }
            else await context.Uniqs.AddAsync(item);
        }

        await context.SaveChangesAsync();
    }

    public Task Upsert(List<Uniq> item)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Uniq item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Uniqs.Update(item);
        await context.SaveChangesAsync();
    }

    public Task UpdateRange(List<Uniq> item)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Uniq>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Uniqs.Include(c => c.explicitModifiers).ToListAsync();
    }

    public Task<Uniq> Get(string name, string detailsId)
    {
        throw new NotImplementedException();
    }
}