using Flipper.Models;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Repository;

public class AccountRepository : IBaseRepository<Account>
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;

    public AccountRepository(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(Account item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Accounts.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task AddRange(List<Account> item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        await context.Accounts.AddRangeAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task Update(Account item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Accounts.Update(item);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRange(List<Account> item)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        context.Accounts.UpdateRange(item);
        await context.SaveChangesAsync();
    }

    public async Task<List<Account>> GetRange()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts.ToListAsync();
    }

    public async Task<Account> Get(string name, string t)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FindAsync<Account>(name);
    }
}