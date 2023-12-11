using System.ComponentModel.DataAnnotations;

namespace Flipper.Models;

public class Description    
{
    public int? id { get; set; }
    [Key]
    public string text { get; set; }
}