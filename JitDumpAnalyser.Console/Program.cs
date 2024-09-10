using JitDumpAnalyser.Core;

var content = File.ReadAllText(args[0]);

var parser = new DumpParser();
var parseResult = parser.Parse(content);

using var streamWriter = new StreamWriter(args[1]);
foreach (var method in parseResult.ParsedMethods)
{
    await SerializeMethodCompilationAsync(streamWriter, method);
}

await streamWriter.FlushAsync();
streamWriter.Close();

static async Task SerializeMethodCompilationAsync(StreamWriter streamWriter, MethodCompilationResult method)
{
    foreach (var phase in method.Phases)
    {
        if (phase.PreInfo != null)
        {
            await streamWriter.WriteAsync(phase.PreInfo);
        }

        await streamWriter.WriteAsync(phase.Content);
        if (phase.PostInfo != null)
        {
            await streamWriter.WriteAsync(phase.PostInfo);
        }
    }
}