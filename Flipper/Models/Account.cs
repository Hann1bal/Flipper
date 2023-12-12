using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flipper.Models;

public class Account
{
    [Key] [Newtonsoft.Json.JsonIgnore] public long id { get; set; }
    [JsonPropertyName("class")] public string AccountName { get; set; }
    [JsonPropertyName("characters")] public List<Character> characters { get; set; } = new List<Character>();
}