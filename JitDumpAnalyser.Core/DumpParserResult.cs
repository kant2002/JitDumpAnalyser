namespace JitDumpAnalyser.Core;

public class DumpParserResult
{
    public List<MethodCompilationResult> ParsedMethods { get; } = new();
}
