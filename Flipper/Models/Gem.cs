namespace Flipper.Models;

public class Gem : BaseItemModel
{
    public int gemLevel { get; set; }
    public int gemQuality { get; set; }
    public bool? corrupted { get; set; }
}