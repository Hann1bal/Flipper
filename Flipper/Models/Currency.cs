using Flipper.Models;
using Newtonsoft.Json;

namespace Flipper.Repository;

public class Currency 
{
    public int id { get; set; }
    public float chaosEquivalent { get; set; }
    public float divineEquivalent { get; set; }
    [JsonProperty("currencyTypeName")]public string? name { get; set; }
    public string detailsId { get; set; }
}