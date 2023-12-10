using Flipper.Models;
using Flipper.Repository;

namespace Flipper.Services;

public class UpdateService
{
    private readonly IBaseRepository<Cards> _cards;
    private readonly IBaseRepository<Gem> _gem;
    private readonly IBaseRepository<Uniq> _uniq;
    private readonly IBaseRepository<Currency> _currency;
    private Currency divine;
    private List<Currency> currency;
    private List<Gem> gems;
    private List<Uniq> uniqs;
    private List<Cards> cards;

    public UpdateService(IBaseRepository<Cards> cards, IBaseRepository<Gem> gem, IBaseRepository<Uniq> uniq,
        IBaseRepository<Currency> currency)
    {
        _cards = cards;
        _gem = gem;
        _uniq = uniq;
        _currency = currency;
    }

    // public async Task Calculator(List<>)
    // {
    //     
    // }
    public async Task UpdateCards(List<Cards> cardsList)
    {
        divine = await _currency.Get("Divine Orb", "divine-orb");
        currency = await _currency.GetRange();
        uniqs = await _uniq.GetRange();
        gems = await _gem.GetRange();
        cards = await _cards.GetRange();
        var cardsId = cards.Select(x => x.idCards).ToList();
        var updateList = cardsList.Where(c => cardsId.Contains(c.idCards)).ToList();
        var insertList = cardsList.Where(c => !cardsId.Contains(c.idCards)).ToList();
        Uniq? uniqItemTmp;
        Gem gemsItemTmp;
        Currency currencyItemTmp;
        Cards cardsItemTmp;

        foreach (var card in updateList)
        {
            var card2 = cards.FirstOrDefault(c => c.idCards == card.idCards);
            var obj = await DescriptionPreprocessor.Process(card.explicitModifiers.First().text);
            card.icon = DescriptionPreprocessor.NameProcessing(card.name);
            card.count = card2.count;
            card.itemFromCardCount = obj.itemFromCardCount;
            card.itemFromCard = obj.itemFromCard;
            card.itemFromCardIsCorrupted = obj.itemFromCardIsCorrupted;
            card.stackSize = card.stackSize == 0 ? 1 : card.stackSize;
            card.fullStackChaosPrice = card2.stackSize * card.chaosValue;
            card.fullStackDivinePrice = card2.stackSize * card.divineValue;
            switch (obj.type)
            {
                case TypeCard.Gem:
                    if(obj.shortName.Contains("Enlighten")) Console.WriteLine("1");
                    gemsItemTmp = gems.Where(c => c.name.Contains(obj.shortName)).Where(s=>s.gemLevel==obj.level).FirstOrDefault();
                    if (gemsItemTmp != null)
                    {
                        card.profitChaos = gemsItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card.profitDivine = gemsItemTmp.divineValue - card2.fullStackDivinePrice;
                        card.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card.pptpd = card2.profitDivinePerTrade / card2.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {card2.name}");
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
                        card.profitChaos = uniqItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card.profitDivine = uniqItemTmp.divineValue - card2.fullStackDivinePrice;
                        card.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card.pptpd = card2.profitDivinePerTrade / card2.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {card2.name}");
                    }

                    break;
                case TypeCard.Divination:
                    cardsItemTmp = cards.Where(c => c.name.Contains(obj.itemFromCard)).FirstOrDefault();
                    if (cardsItemTmp != null)
                    {
                        card.profitChaos = cardsItemTmp.chaosValue - card2.fullStackChaosPrice;
                        card.profitDivine = cardsItemTmp.divineValue - card2.fullStackDivinePrice;
                        card.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card.pptpd = card2.profitDivinePerTrade / card2.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {card2.name}");
                    }

                    break;
                case TypeCard.Currency:
                    currencyItemTmp = currency.FirstOrDefault(c => c.name == obj.itemFromCard);
                    if (currencyItemTmp != null)
                    {
                        card.profitChaos = currencyItemTmp.chaosEquivalent - card2.fullStackChaosPrice;
                        card.profitDivine = currencyItemTmp.chaosEquivalent / divine.chaosEquivalent -
                                            card2.fullStackDivinePrice;
                        card.profitChaosPerTrade = card2.profitChaos / card2.count;
                        card.profitDivinePerTrade = card2.profitDivine / card2.count;
                        card.pptpc = card2.profitChaosPerTrade / card2.chaosValue;
                        card.pptpd = card2.profitDivinePerTrade / card2.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"{obj.itemFromCard} - {card2.name}");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        await _cards.UpdateRange(updateList);
        foreach (var card in insertList)
        {
            var obj = await DescriptionPreprocessor.Process(card.explicitModifiers.First().text);
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
                    else
                    {
                        Console.WriteLine($"New Item = {obj.itemFromCard} - {card.name}");
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
                    else
                    {
                        Console.WriteLine($"New Item = {obj.itemFromCard} - {card.name}");
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
                    else
                    {
                        Console.WriteLine($"New Item = {obj.itemFromCard} - {card.name}");
                    }

                    break;
                case TypeCard.Currency:
                    var itemCurrency = currency.FirstOrDefault(c => c.name == obj.itemFromCard);
                    if (itemCurrency != null)
                    {
                        card.profitChaos = itemCurrency.chaosEquivalent - card.fullStackChaosPrice;
                        card.profitDivine = itemCurrency.chaosEquivalent / divine.chaosEquivalent -
                                            card.fullStackDivinePrice;
                        card.profitChaosPerTrade = card.profitChaos / card.count;
                        card.profitDivinePerTrade = card.profitDivine / card.count;
                        card.pptpc = card.profitChaosPerTrade / card.chaosValue;
                        card.pptpd = card.profitDivinePerTrade / card.chaosValue;
                    }
                    else
                    {
                        Console.WriteLine($"New Item {obj.itemFromCard} - {card.name}");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        await _cards.AddRange(insertList);
    }
}