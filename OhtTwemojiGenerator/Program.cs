// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OhtTwemojiGenerator;
using RestSharp;

var result = new List<OhtIconCategeroy>();

// Get source list
var client = new RestClient("https://raw.githubusercontent.com/github/gemoji/master/db/emoji.json");
var sourceEmojis = client.Get<SourceEmoji[]>(new RestRequest());

// Create all the categories
result.AddRange(sourceEmojis.Select(x => x.category).Distinct().Select(x => new OhtIconCategeroy()
{
    Name = x
}).ToList());

// Add all the emotions to all the categories
foreach (var category in result)
{
    var emojis = sourceEmojis.Where(x => x.category == category.Name);
    foreach (var emoji in emojis)
    {
        List<string> names = new();
        names.Add(emoji.description);
        names.AddRange(emoji.aliases);
        names.AddRange(emoji.tags);
        category.Icons.Add(new OhtIcon()
        {
            Names = names.ToArray(),
            Url = $"https://cdn.jsdelivr.net/gh/twitter/twemoji@14.0.2/assets/svg/{ConvertEmojiToCodepoint(emoji.emoji)}.svg".ToLower()
        });
    }
}
Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));

static string ConvertEmojiToCodepoint(string input)
{
    StringBuilder output = new StringBuilder();

    for (int i = 0; i < input.Length; i++)
    {
        char c = input[i];

        if (char.IsHighSurrogate(c) && i < input.Length - 1 && char.IsLowSurrogate(input[i + 1]))
        {
            int codepoint = char.ConvertToUtf32(c, input[i + 1]);
            output.Append($"{codepoint:X4}{(i == input.Length - 2 ? "" : "-")}");
            i++; // Skip the low surrogate
        }
        else if (!char.IsLowSurrogate(c))
        {
            output.Append(c);
        }
    }

    return output.ToString().ToUpper();
}