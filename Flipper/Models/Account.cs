using System.ComponentModel.DataAnnotations;

namespace Flipper.Models;

public class Account
{

    public int Rank { get; set; }
    [Key]
    public string AccountName { get; set; }
    public List<Character> Characters { get; set; } = new List<Character>();
}