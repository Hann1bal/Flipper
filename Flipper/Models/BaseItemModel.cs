using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Flipper.Models;

public class BaseItemModel
{
    [Key] [JsonProperty("id")] public int? idCards { get; set; }
    public string name { get; set; }
    public Uri icon { get; set; }
    public float chaosValue { get; set; }
    public float divineValue { get; set; }
    public string? baseType { get; set; }
    public List<Description> explicitModifiers { get; set; }
    public int count { get; set; }
}