using System.Diagnostics;

namespace JitDumpAnalyser.Core;

[DebuggerDisplay("Phase = {Name}")]
public class PhaseInformation
{
    public PhaseInformation(string name, string content)
    {
        Name = name;
        Content = content;
    }

    public string Name { get; }

    public string Content { get; internal set; }

    public string? PreInfo { get; internal set; }
    public string? PostInfo { get; internal set; }
    public bool NoChanges { get; init; }

    public List<MethodBody> MethodsDefinitions { get; } = new();
}