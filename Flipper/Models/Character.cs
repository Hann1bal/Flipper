using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flipper.Models;

public class Character
{
    [Key] [JsonPropertyName("id")] public string id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("class")] public string Class { get; set; }
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("experience")] public long Experiance { get; set; }
}