using System.Text.RegularExpressions;
using Flipper.Models;

namespace Flipper.Services;

public class DescriptionPreprocessor
{
    public async static Task<TransferDto> Process(string description)
    {
        bool corrupted = description.Contains("<corrupted>");
        bool isCurrency = false;
        var descript = await DescriptionPreprocessor.PreProcess(description.Replace("\n", ""));
        Regex regex = new Regex(@"{(.*?)}");
        Regex regexstart = new Regex(@"<(.*?)>");
        Regex count = new Regex(@"\d*x.*?");
        Regex linked = new Regex(@"\w*-Link..");
        MatchCollection types = regexstart.Matches(descript);
        MatchCollection items = regex.Matches(descript);
        MatchCollection links = linked.Matches(descript);
        string type = "";
        string item = "";
        bool link = false;
        if(types.Any()) type = types.First().Value.Replace("<", "").Replace(">", "");
        if(items.Any()) item = items.First().Value.Replace("{", "").Replace("}", "");
        if(links.Any()) link = true;
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

            isCurrency = true;
        }

        return new TransferDto
            {links= link,tags = type, itemFromCard = cutint, itemFromCardIsCorrupted = corrupted, itemFromCardCount = countItem, isCurrency = isCurrency};
    }

    public async static Task<string> PreProcess(string description)
    {
        if (description.StartsWith("<size:"))
        {
            Regex regex = new Regex(@"{(.*?)}}");
            MatchCollection items = regex.Matches(description.Replace("\n", ""));
            var item = items.First().Value.Substring(1, items.First().Value.Length - 2);
            return item;
        }

        return description;
    }
}