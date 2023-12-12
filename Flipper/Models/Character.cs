using System.ComponentModel.DataAnnotations;

namespace Flipper.Models;

public class Character
{
    [Key] public string Name { get; set; }
    public string Class { get; set; }
    public int Level { get; set; }
    public long Experiance { get; set; }

}