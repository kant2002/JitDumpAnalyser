namespace JitDumpAnalyser.Core;

public class PhaseInformation
{
    public PhaseInformation(string name, string content)
    {
        Name = name;
        Content = content;
    }

    public string Name { get; }

    public string Content { get; }

    public string? PreInfo { get; internal set; }
    public string? PostInfo { get; internal set; }
    public bool NoChanges { get; init; }
}