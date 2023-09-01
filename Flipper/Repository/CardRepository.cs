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
        var divine =
            await context.Currency.FirstOrDefaultAsync(c => c.name == "Divine Orb" || c.detailsId == "divine-orb");

        foreach (var item in items)
        {
            var obj = await DescriptionPreprocessor.Process(item.explicitModifiers.First().text);
            if (context.Cards.Include(c => c.explicitModifiers).Any(c => c.idCards == item.idCards))
            {
                var entity = await context.Cards.FirstOrDefaultAsync(c => c.idCards == item.idCards);
                var urlname = item.name;
                if (urlname.Contains(" of "))
                    urlname = urlname.Replace(" of ", " Of ");
                if (urlname.Contains("'"))
                    urlname = urlname.Replace("'", "");
                if (urlname.Contains(" and "))
                    urlname = urlname.Replace(" and ", " And ");
                if (urlname.Contains(" the "))
                    urlname = urlname.Replace(" the ", " The ");
                if (urlname.Contains(" in "))
                    urlname = urlname.Replace(" in ", "In");
                if (urlname.Contains(" is "))
                    urlname = urlname.Replace(" is ", "Is");
                if (urlname.Contains(" to "))
                    urlname = urlname.Replace(" to ", " To ");
                entity.icon = new Uri(
                    $"https://web.poecdn.com/image/divination-card/{urlname.Replace(" ", "")}.png");
                entity.name = item.name;
                entity.chaosValue = item.chaosValue;
                entity.divineValue = item.divineValue;
                entity.baseType = item.baseType;
                entity.explicitModifiers = item.explicitModifiers;
                entity.count = entity.count;
                entity.itemFromCardCount = obj.itemFromCardCount;
                entity.itemFromCard = obj.itemFromCard;
                entity.itemFromCardIsCorrupted = obj.itemFromCardIsCorrupted;
                entity.stackSize = item.stackSize==0?1:item.stackSize;
                entity.fullStackChaosPrice = entity.stackSize * item.chaosValue;
                entity.fullStackDivinePrice = entity.stackSize * item.divineValue;
                if (context.Uniqs.Any(c => c.name == obj.itemFromCard))
                {
                    List<Uniq?> itemUniq = await context.Uniqs.Where(c => c.name.Contains(obj.itemFromCard)).ToListAsync();
                    Uniq? itemb;
                    if (obj.links ) itemb = itemUniq.MaxBy(c=>c.links);
                    else itemb = itemUniq.Any(c => c.links == null) ? itemUniq.FirstOrDefault(c => c.links == null) : itemUniq.MinBy(c => c.links);
                    entity.profitChaos = itemb.chaosValue - entity.fullStackChaosPrice;
                    entity.profitDivine = itemb.divineValue - entity.fullStackDivinePrice;
                    entity.profitChaosPerTrade = entity.profitChaos / entity.count;
                    entity.profitDivinePerTrade = entity.profitDivine / entity.count;
                    entity.pptpc = entity.profitChaosPerTrade / entity.chaosValue;
                    entity.pptpd = entity.profitDivinePerTrade / entity.chaosValue;
                }
                if (obj is { isCurrency: true})
                {
                    var itemCurrency = await context.Currency.FirstOrDefaultAsync(c => c.name == obj.itemFromCard);
                    if (itemCurrency != null)
                    {
                        entity.profitChaos = itemCurrency.chaosEquivalent - entity.fullStackChaosPrice;
                        entity.profitDivine = itemCurrency.chaosEquivalent / divine.chaosEquivalent - entity.fullStackDivinePrice;
                        entity.profitChaosPerTrade = entity.profitChaos / entity.count;
                        entity.profitDivinePerTrade = entity.profitDivine / entity.count;
                        entity.pptpc = entity.profitChaosPerTrade / entity.chaosValue;
                        entity.pptpd = entity.profitDivinePerTrade / entity.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {entity.name}");
                    }
                }

                context.Cards.Update(entity);
            }
            else
            {
                var urlname = item.name;
                if (urlname.Contains(" of "))
                    urlname = urlname.Replace(" of ", "Of");
                if (urlname.Contains("'"))
                    urlname = urlname.Replace("'", "");
                if (urlname.Contains(" and "))
                    urlname = urlname.Replace(" and ", "And");
                if (urlname.Contains(" the "))
                    urlname = urlname.Replace(" the ", "The");
                if (urlname.Contains(" in "))
                    urlname = urlname.Replace(" in ", "In");
                if (urlname.Contains(" is "))
                    urlname = urlname.Replace(" is ", "Is");
                if (urlname.Contains(" to "))
                    urlname = urlname.Replace(" to ", " To ");
                item.icon = new Uri(
                    $"https://web.poecdn.com/image/divination-card/{urlname.Replace(" ", "")}.png");
                item.itemFromCardCount = obj.itemFromCardCount;
                item.itemFromCard = obj.itemFromCard;
                item.itemFromCardIsCorrupted = obj.itemFromCardIsCorrupted;
                item.stackSize = item.stackSize==0?1:item.stackSize;
                item.fullStackChaosPrice = item.stackSize * item.chaosValue;
                item.fullStackDivinePrice = item.stackSize * item.divineValue;
                // item.tags = new List<string> { obj.tags };
                if (context.Uniqs.Any(c => c.name == obj.itemFromCard))
                {
                    List<Uniq?> itemUniq = await context.Uniqs.Where(c => c.name.Contains(obj.itemFromCard)).ToListAsync();
                    Uniq? itemb;
                    if (obj.links ) itemb = itemUniq.MaxBy(c=>c.links);
                    else itemb = itemUniq.Any(c => c.links == null) ? itemUniq.FirstOrDefault(c => c.links == null) : itemUniq.MinBy(c => c.links);
                    item.profitChaos = itemb.chaosValue - item.fullStackChaosPrice;
                    item.profitDivine = itemb.divineValue - item.fullStackDivinePrice;
                    item.profitChaosPerTrade = item.profitChaos / item.count;
                    item.profitDivinePerTrade = item.profitDivine / item.count;
                    item.pptpc = item.profitChaosPerTrade / item.chaosValue;
                    item.pptpd = item.profitDivinePerTrade / item.chaosValue;
                }
                if (obj is { isCurrency: true, itemFromCardCount: > 0 })
                {
                    var itemCurrency = await context.Currency.FirstOrDefaultAsync(c => c.name == obj.itemFromCard);
                    if (itemCurrency != null)
                    {
                        item.profitChaos = itemCurrency.chaosEquivalent - item.fullStackChaosPrice;
                        item.profitDivine = itemCurrency.chaosEquivalent / divine.chaosEquivalent - item.fullStackDivinePrice;
                        item.profitChaosPerTrade = item.profitChaos / item.count;
                        item.profitDivinePerTrade = item.profitDivine / item.count;
                        item.pptpc = item.profitChaosPerTrade / item.chaosValue;
                        item.pptpd = item.profitDivinePerTrade / item.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {item.name}");
                    }
                }

                await context.Cards.AddAsync(item);
            }
        }

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
}