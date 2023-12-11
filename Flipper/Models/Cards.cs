using System.ComponentModel.DataAnnotations;

namespace Flipper.Models;

public class Cards : BaseItemModel
{
    public int stackSize { get; set; }
    public string? itemFromCard { get; set; }
    public string? flavourText { get; set; }
    public bool? itemFromCardIsCorrupted { get; set; }
    public int? itemFromCardCount { get; set; }
    
    public float fullStackChaosPrice { get; set; }
    
    public float fullStackDivinePrice { get; set; }
    public float? profitChaos { get; set; }
    public float? profitDivine { get; set; }
    public float? profitChaosPerTrade { get; set; }
    public float? profitDivinePerTrade { get; set; }
    public float? pptpc { get; set; }
    public float? pptpd { get; set; }
    // public List<string>? tags { get; set; }
}