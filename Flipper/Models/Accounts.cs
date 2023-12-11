namespace Flipper.Models;

public class Accounts
{
    public int Rank { get; set; }
    public string Account { get; set; }
    public List<string> Characters { get; set; } = new List<string>();
    public string Class { get; set; }
    public int Level { get; set; }
    public long Experiance { get; set; }
}