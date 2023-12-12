using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Flipper.Models;

public class AccountModel
{
    [JsonIgnore] public long id { get; set; }
    [JsonPropertyName("name")] public string name { get; set; }
    [JsonPropertyName("realm")] public string realm { get; set; }
    [JsonPropertyName("twitch")] public Twitch? twitch { get; set; }
}