namespace JitDumpAnalyser.Core;

public class DumpParser
{
    public DumpParserResult Parse(string content)
    {
        const string StartCompilingMarker = "****** START compiling ";
        const string EndCompilingMarker = "****** DONE compiling ";
        const string MethodHashMarker = " (MethodHash=";

        var seekIndex = 0;
        var result = new DumpParserResult();
        while (seekIndex < content.Length)
        {
            var startIndex = content.IndexOf(StartCompilingMarker, seekIndex);
            if (startIndex < 0)
            {
                return result;
            }

            var endIndex = content.IndexOf(EndCompilingMarker, startIndex + StartCompilingMarker.Length);
            var spaceMarker = content.IndexOf(' ', startIndex + StartCompilingMarker.Length);
            var methodName = content[(startIndex + StartCompilingMarker.Length)..spaceMarker];
            var hashEndIndex = content.IndexOf(")", spaceMarker + MethodHashMarker.Length);
            var methodHash = uint.Parse(content.Substring(spaceMarker + MethodHashMarker.Length, hashEndIndex - spaceMarker - MethodHashMarker.Length), System.Globalization.NumberStyles.HexNumber);
            string methodContent = content.Substring(startIndex, endIndex - startIndex + methodName.Length + EndCompilingMarker.Length);
            var methodCompilationResult = new MethodCompilationResult(methodName, methodContent)
            {
                MethodHash = methodHash,
            };
            ParsePhases(methodContent, methodCompilationResult.Phases);
            result.ParsedMethods.Add(methodCompilationResult);
            seekIndex = endIndex + EndCompilingMarker.Length + methodName.Length ;
        }

        return result;
    }

    private void ParsePhases(string content, IList<PhaseInformation> phases)
    {
        const string StartMarker = "*************** Starting PHASE ";
        const string EndMarker = "*************** Finishing PHASE ";
        const string NoChangesMarker = " [no changes]";

        var seekIndex = 0;
        while (seekIndex < content.Length)
        {
            var startIndex = content.IndexOf(StartMarker, seekIndex);
            if (startIndex < 0)
            {
                return;
            }

            var endIndex = content.IndexOf(EndMarker, startIndex + StartMarker.Length);
            var spaceMarker = content.IndexOf("\r\n", startIndex + StartMarker.Length);
            var phaseName = content[(startIndex + StartMarker.Length)..spaceMarker];
            seekIndex = endIndex + EndMarker.Length + phaseName.Length;
            string phaseContent;
            bool noChanges = false;
            if (content.IndexOf(NoChangesMarker, endIndex) == seekIndex)
            {
                noChanges = true;
                seekIndex += NoChangesMarker.Length;
                phaseContent = content[startIndex..seekIndex];
            }
            else
            {
                phaseContent = content[startIndex..(endIndex + phaseName.Length + EndMarker.Length)];
            }

            var phaseInformation = new PhaseInformation(phaseName, phaseContent)
            {
                NoChanges = noChanges,
            };

            phases.Add(phaseInformation);
        }
    }
}