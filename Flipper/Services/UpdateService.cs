using Flipper.Models;
using Flipper.Repository;
using Microsoft.EntityFrameworkCore;

namespace Flipper.Services;

public class UpdateService
{
    private readonly IDbContextFactory<FlipperContext> _contextFactory;
    private Currency? divine;
    private List<Currency> currency;
    private List<Gem> gems;
    private List<Uniq> uniqs;
    private List<Cards> cards;

    public UpdateService(IDbContextFactory<FlipperContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task UpdateCards(List<Cards> cardsList)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        divine = await context.Currency.FirstOrDefaultAsync(c => c.name == "Divine Orb" || c.detailsId == "divine-orb");
        currency = await context.Currency.ToListAsync();
        uniqs = await context.Uniqs.ToListAsync();
        gems = await context.Gem.ToListAsync();
        cards = await context.Cards.ToListAsync();
        var cardsId = cards.Select(x => x.idCards).ToList();
        var updateList = cardsList.Where(c => cardsId.Contains(c.idCards)).ToList();
        var insertList = cardsList.Where(c => !cardsId.Contains(c.idCards)).ToList();
        Uniq? uniqItemTmp;
        Gem gemsItemTmp;
        Currency currencyItemTmp;
        Cards cardsItemTmp;

        foreach (var card in updateList)
        {
            var card2 = await context.Cards.FirstOrDefaultAsync(c => c.idCards == card.idCards);
            var obj = await DescriptionPreprocessor.Process(card.explicitModifiers.First().text);
            var t = DescriptionPreprocessor.FlowerTextProccessor(card.flavourText!.Replace("\n", " "));
            card2.flavourText = t;
            card2.count = card.count;
            card2.icon = DescriptionPreprocessor.NameProcessing(card.name);
            card2.itemFromCardCount = obj.itemFromCardCount;
            card2.itemFromCard = obj.itemFromCard;
            card2.itemFromCardIsCorrupted = obj.itemFromCardIsCorrupted;
            card2.stackSize = card.stackSize == 0 ? 1 : card.stackSize;
            card2.flavourText = card.flavourText;
            card2.fullStackChaosPrice = card.stackSize * card2.chaosValue;
            card2.fullStackDivinePrice = card.stackSize * card2.divineValue;
            card2.explicitModifiers = card2.explicitModifiers;
            switch (obj.type)
            {
                case TypeCard.Gem:
                    gemsItemTmp = gems.Where(c => c.name.Contains(obj.shortName)).Where(s => s.gemLevel == obj.level)
                        .FirstOrDefault();
                    if (gemsItemTmp != null)
                    {

                        card2.profitChaos = gemsItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card2.profitDivine = gemsItemTmp.divineValue - card2.fullStackDivinePrice;
                        card2.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card2.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card2.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card2.pptpd = card2.profitDivinePerTrade / card2.chaosValue;

                    }
                    break;
                case TypeCard.Unique:
                    if (obj.links)
                        uniqItemTmp = uniqs.Where(c => c.name.Contains(obj.itemFromCard)).MaxBy(c => c.links);
                    else
                        uniqItemTmp = uniqs.Where(c => c.name.Contains(obj.itemFromCard)).Any(c => c.links == null)
                            ? uniqs.Where(c => c.name.Contains(obj.itemFromCard)).FirstOrDefault(c => c.links == null)
                            : uniqs.Where(c => c.name.Contains(obj.itemFromCard)).MinBy(c => c.links);
                    if (uniqItemTmp != null)
                    {
                        card2.profitChaos = uniqItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card2.profitDivine = uniqItemTmp.divineValue - card2.fullStackDivinePrice;
                        card2.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card2.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card2.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card2.pptpd = card2.profitDivinePerTrade / card2.chaosValue;

                    }

                    break;
                case TypeCard.Divination:
                    cardsItemTmp = cards.Where(c => c.name.Contains(obj.itemFromCard)).FirstOrDefault();
                    if (cardsItemTmp != null)
                    {
                        card2.profitChaos = cardsItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card2.profitDivine = cardsItemTmp.divineValue - card2.fullStackDivinePrice;
                        card2.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card2.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card2.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card2.pptpd = card2.profitDivinePerTrade / card2.chaosValue;

                    }
                    break;
                case TypeCard.Currency:
                    currencyItemTmp = currency.FirstOrDefault(c => c.name == obj.itemFromCard);
                    if (currencyItemTmp != null)
                    {
                        card2.profitChaos = currencyItemTmp.chaosEquivalent - card2.fullStackChaosPrice;
                        card2.profitDivine = currencyItemTmp.chaosEquivalent / divine.chaosEquivalent -
                                            card2.fullStackDivinePrice;
                        card2.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card2.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card2.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card2.pptpd = card2.profitDivinePerTrade / card2.chaosValue;

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            context.Cards.Update(card2);
            await context.SaveChangesAsync();

        }

        foreach (var card in insertList)
        {
            var obj = await DescriptionPreprocessor.Process(card.explicitModifiers.First().text);
            var t = DescriptionPreprocessor.FlowerTextProccessor(card.flavourText.Replace("\n", " "));

            card.flavourText = t;
            card.icon = DescriptionPreprocessor.NameProcessing(card.name);
            card.itemFromCardCount = obj.itemFromCardCount;
            card.itemFromCard = obj.itemFromCard;
            card.itemFromCardIsCorrupted = obj.itemFromCardIsCorrupted;
            card.stackSize = card.stackSize == 0 ? 1 : card.stackSize;
            card.fullStackChaosPrice = card.stackSize * card.chaosValue;
            card.fullStackDivinePrice = card.stackSize * card.divineValue;
            switch (obj.type)
            {
                case TypeCard.Gem:
                    gemsItemTmp = gems.Where(c => c.name.Contains(obj.shortName)).FirstOrDefault();
                    if (gemsItemTmp != null)
                    {
                        card.profitChaos = gemsItemTmp.chaosValue - card.fullStackChaosPrice;
                        card.profitDivine = gemsItemTmp.divineValue - card.fullStackDivinePrice;
                        card.profitChaosPerTrade = card.profitChaos / card.count;
                        card.profitDivinePerTrade = card.profitDivine / card.count;
                        card.pptpc = card.profitChaosPerTrade / card.chaosValue;
                        card.pptpd = card.profitDivinePerTrade / card.chaosValue;
                    }

                    break;
                case TypeCard.Unique:
                    if (obj.links)
                        uniqItemTmp = uniqs.Where(c => c.name.Contains(obj.itemFromCard)).MaxBy(c => c.links);
                    else
                        uniqItemTmp = uniqs.Where(c => c.name.Contains(obj.itemFromCard)).Any(c => c.links == null)
                            ? uniqs.Where(c => c.name.Contains(obj.itemFromCard)).FirstOrDefault(c => c.links == null)
                            : uniqs.Where(c => c.name.Contains(obj.itemFromCard)).MinBy(c => c.links);
                    if (uniqItemTmp != null)
                    {
                        card.profitChaos = uniqItemTmp.chaosValue - card.fullStackChaosPrice;
                        card.profitDivine = uniqItemTmp.divineValue - card.fullStackDivinePrice;
                        card.profitChaosPerTrade = card.profitChaos / card.count;
                        card.profitDivinePerTrade = card.profitDivine / card.count;
                        card.pptpc = card.profitChaosPerTrade / card.chaosValue;
                        card.pptpd = card.profitDivinePerTrade / card.chaosValue;
                    }

                    break;
                case TypeCard.Divination:
                    cardsItemTmp = cards.Where(c => c.name.Contains(obj.itemFromCard)).FirstOrDefault();
                    if (cardsItemTmp != null)
                    {
                        card.profitChaos = cardsItemTmp.chaosValue - card.fullStackChaosPrice;
                        card.profitDivine = cardsItemTmp.divineValue - card.fullStackDivinePrice;
                        card.profitChaosPerTrade = card.profitChaos / card.count;
                        card.profitDivinePerTrade = card.profitDivine / card.count;
                        card.pptpc = card.profitChaosPerTrade / card.chaosValue;
                        card.pptpd = card.profitDivinePerTrade / card.chaosValue;
                    }

                    break;
                case TypeCard.Currency:
                    var itemCurrency = currency.FirstOrDefault(c => c.name == obj.itemFromCard);
                    if (itemCurrency != null)
                    {
                        card.profitChaos = itemCurrency.chaosEquivalent - card.fullStackChaosPrice;
                        card.profitDivine = itemCurrency.chaosEquivalent / divine.chaosEquivalent - card.fullStackDivinePrice;
                        card.profitChaosPerTrade = card.profitChaos / card.count;
                        card.profitDivinePerTrade = card.profitDivine / card.count;
                        card.pptpc = card.profitChaosPerTrade / card.chaosValue;
                        card.pptpd = card.profitDivinePerTrade / card.chaosValue;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        await context.Cards.AddRangeAsync(insertList);
        await context.SaveChangesAsync();
    }
}