namespace Flipper.Models;

public enum TypeCard:byte
{
    Gem =1, 
    Unique = 2,
    Divination = 3,
    Currency = 4
}

public class TransferDto
{

    public string? itemFromCard { get; set; }
    public string? shortName { get; set; }
    public bool? itemFromCardIsCorrupted { get; set; }
    public TypeCard type { get; set; }
    public int? itemFromCardCount { get; set; }
    public string tags { get; set; }
    public bool links { get; set; }
    public int? level { get; set; }
}