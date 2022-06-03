using System.Diagnostics;

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
            string methodContent;
            if (endIndex != -1)
            {
                seekIndex = endIndex + EndCompilingMarker.Length + methodName.Length;
                methodContent = content.Substring(startIndex, endIndex - startIndex + methodName.Length + EndCompilingMarker.Length);
            }
            else
            {
                seekIndex = content.Length;
                methodContent = content.Substring(startIndex);
            }

            var methodCompilationResult = new MethodCompilationResult(methodName, methodContent)
            {
                MethodHash = methodHash,
            };
            ParsePhases(methodContent, methodCompilationResult.Phases);
            result.ParsedMethods.Add(methodCompilationResult);
        }

        return result;
    }

    private void ParsePhases(string content, IList<PhaseInformation> phases)
    {
        const string StartMarker = "*************** Starting PHASE ";
        const string EndMarker = "*************** Finishing PHASE ";
        const string NoChangesMarker = " [no changes]";

        var seekIndex = 0;
        PhaseInformation? lastPhase = null;
        while (seekIndex < content.Length)
        {
            var startIndex = content.IndexOf(StartMarker, seekIndex);
            if (startIndex < 0)
            {
                if (lastPhase != null)
                {
                    var postInit = content[seekIndex..];
                    lastPhase.PostInfo = postInit;
                }

                return;
            }

            var preInit = content[seekIndex..startIndex];
            if (preInit.Length <= 4)
            {
                preInit = null;
            }
            else
            {
                if (preInit.StartsWith("\r\nTrees after") && lastPhase is not null)
                {
                    var treesBeforeIndex = preInit.IndexOf("Trees before");
                    if (treesBeforeIndex == -1)
                    {
                        lastPhase.PostInfo = preInit;
                        preInit = null;
                    }
                    else
                    {
                        lastPhase.PostInfo = preInit[0..treesBeforeIndex];
                        preInit = preInit[treesBeforeIndex..];
                    }
                }
            }

            var endIndex = content.IndexOf(EndMarker, startIndex + StartMarker.Length);
            var spaceMarker = content.IndexOf("\r\n", startIndex + StartMarker.Length);
            var phaseName = content[(startIndex + StartMarker.Length)..spaceMarker];
            string phaseContent;
            bool noChanges = false;
            if (endIndex == -1)
            {
                phaseContent = content[startIndex..];
                seekIndex = content.Length;
            }
            else
            {
                seekIndex = endIndex + EndMarker.Length + phaseName.Length;
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
            }

            var phaseInformation = new PhaseInformation(phaseName, phaseContent)
            {
                NoChanges = noChanges,
                PreInfo = preInit,
            };

            phases.Add(phaseInformation);
            lastPhase = phaseInformation;
        }
    }
}