// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net;
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
            Url = GetTwemojiUrl(emoji.emoji)
        });
    }
}

result.SelectMany(x => x.Icons).Where(x => x.Url.Contains("-fe0f")).ToList().ForEach(x =>
{
    var testClient = new RestClient(x.Url);
    var test = testClient.Get(new RestRequest());

    if (test.StatusCode != HttpStatusCode.OK)
    {
        x.Url = x.Url.Replace("-fe0f", "");
        Console.WriteLine("Fixed " + x.Url);
    }
});

Console.WriteLine(JsonSerializer.Serialize(result));

static string GetTwemojiUrl(string emoji)
{
    StringBuilder unicodeBuilder = new StringBuilder();
        
    for (int i = 0; i < emoji.Length; i++)
    {
        int codepoint = char.ConvertToUtf32(emoji, i);
        if (char.IsSurrogate(emoji[i]))
        {
            i++; // Skip the second part of the surrogate pair
        }

        if (unicodeBuilder.Length > 0)
        {
            unicodeBuilder.Append("-");
        }
            
        unicodeBuilder.AppendFormat("{0:x4}", codepoint);
    }

    return $"https://cdn.jsdelivr.net/gh/twitter/twemoji@latest/assets/svg/{unicodeBuilder}.svg";
}