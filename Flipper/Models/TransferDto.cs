namespace Flipper.Models;

public class TransferDto
{
    public string? itemFromCard { get; set; }
    public bool? itemFromCardIsCorrupted { get; set; }
    public bool isCurrency { get; set; }
    public int? itemFromCardCount { get; set; }
    public string tags { get; set; }
    public bool links { get; set; }
}