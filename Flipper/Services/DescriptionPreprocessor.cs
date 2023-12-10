using System.Text.RegularExpressions;
using Flipper.Models;

namespace Flipper.Services;

public class DescriptionPreprocessor
{
    public static async Task<TransferDto> Process(string description)
    {
        bool corrupted = description.Contains("<corrupted>");
        TypeCard typeCard = TypeCard.Unique;
        var descript = await DescriptionPreprocessor.PreProcess(description.Replace("\n", ""));
        Regex regex = new Regex(@"{(.*?)}");
        Regex regexstart = new Regex(@"<(.*?)>");
        Regex count = new Regex(@"\d*x.*?");
        Regex level = new Regex(@"(Level \d+)");
        Regex linked = new Regex(@"\w*-Link..");
        MatchCollection types = regexstart.Matches(descript);
        MatchCollection items = regex.Matches(descript);
        MatchCollection links = linked.Matches(descript);
        string type = "";
        string shortName = "";
        string item = "";
        bool link = false;
        int levelItemGem = 1;
        if (types.Any()) type = types.First().Value.Replace("<", "").Replace(">", "");
        if (items.Any()) item = items.First().Value.Replace("{", "").Replace("}", "");
        if (links.Any()) link = true;
        int countItem = 1;
        string cutint = item;
        if (type.StartsWith(@"currency"))
        {
            var t = count.Matches(item);
            if (t.Any())
            {
                var number = t.First().Value.Replace("x", "");
                int.TryParse(number, out countItem);
                cutint = item.Replace(t.First().Value + " ", "");
            }

            typeCard = TypeCard.Currency;
        }

        if (type.StartsWith(@"gem"))
        {
            var t = count.Matches(item);
            if (t.Any())
            {
                var number = t.First().Value.Replace("x", "");
                int.TryParse(number, out countItem);
                cutint = item.Replace(t.First().Value + " ", "");
            }

            var levelGem = level.Matches(item);
            if (levelGem.Any())
            {
                var p = levelGem.ToList().First().Value.Split(" ")[1];
                shortName= item.Split(levelGem.ToList().First().Value)[1].Trim();
                Int32.TryParse(p, out levelItemGem);
            }

            typeCard = TypeCard.Gem;
        }

        if (type.StartsWith(@"divination"))
        {
            var t = count.Matches(item);
            if (t.Any())
            {
                var number = t.First().Value.Replace("x", "");
                int.TryParse(number, out countItem);
                cutint = item.Replace(t.First().Value + " ", "");
            }

            typeCard = TypeCard.Divination;
        }

        if (type.StartsWith(@"unique"))
        {
            var t = count.Matches(item);
            if (t.Any())
            {
                var number = t.First().Value.Replace("x", "");
                int.TryParse(number, out countItem);
                cutint = item.Replace(t.First().Value + " ", "");
            }

            typeCard = TypeCard.Unique;
        }

        return new TransferDto
        {
            links = link, tags = type, itemFromCard = cutint, itemFromCardIsCorrupted = corrupted,
            itemFromCardCount = countItem, type = typeCard, level = levelItemGem, shortName = shortName
        };
    }

    private async static Task<string> PreProcess(string description)
    {
        if (!description.StartsWith("<size:")) return description;
        var items = new Regex(@"{(.*?)}}").Matches(description.Replace("\n", ""));
        var item = items.First().Value.Substring(1, items.First().Value.Length - 2);
        return item;
    }

    public static Uri NameProcessing(string name)
    {
        var urlName = name;
        urlName = urlName.Contains(" of ") ? urlName.Replace(" of ", " Of ") : urlName;
        urlName = urlName.Contains("'") ? urlName.Replace("'", "") : urlName;
        urlName = urlName.Contains(" and ") ? urlName.Replace(" and ", " And ") : urlName;
        urlName = urlName.Contains(" the ") ? urlName.Replace(" the ", " The ") : urlName;
        urlName = urlName.Contains(" in ") ? urlName.Replace(" in ", "In") : urlName;
        urlName = urlName.Contains(" is ") ? urlName.Replace(" is ", "Is") : urlName;
        urlName = urlName.Contains(" to ") ? urlName.Replace(" to ", " To ") : urlName;
        return new Uri($"https://web.poecdn.com/image/divination-card/{urlName.Replace(" ", "")}.png");
    }
}