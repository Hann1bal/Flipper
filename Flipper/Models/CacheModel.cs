namespace Flipper.Models;

public class CacheModel
{
    public List<Entrie> entries { get; set; }
    public int total { get; set; }
    public DateTime cached_since { get; set; }
}