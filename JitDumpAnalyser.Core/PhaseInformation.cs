namespace JitDumpAnalyser.Core;

public class PhaseInformation
{
    public PhaseInformation(string name)
    {
        Name = name;
    }

    public string Name { get; internal set; }

    public string Content { get; internal init; }

    public string? PreInfo { get; internal set; }
    public string? PostInfo { get; internal set; }
    public bool NoChanges { get; init; }
}