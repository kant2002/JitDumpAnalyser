namespace JitDumpAnalyser.Core;

public class MethodCompilationResult
{
    public MethodCompilationResult(string methodName, string content)
    {
        MethodName = methodName;
        Content = content;
    }

    public string MethodName { get; }

    public string Content { get; }

    public uint MethodHash { get; set; }

    public List<PhaseInformation> Phases { get; } = new();
}
