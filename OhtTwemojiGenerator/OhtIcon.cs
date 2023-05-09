namespace OhtTwemojiGenerator;

public class OhtIcon
{
    public string[] Names { get; set; }
    public string Url { get; set; }
}

public class OhtIconCategeroy
{
    public string Name { get; set; }
    public List<OhtIcon> Icons = new();
}