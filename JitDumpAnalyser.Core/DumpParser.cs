using System;
using System.Reflection.Metadata;

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
            foreach (var phase in methodCompilationResult.Phases)
            {
                ParseBasicBlocks(phase);
            }

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
                if (lastPhase is not null)
                {
                    if (lastPhase.PostInfo is not null)
                    {
                        lastPhase.PostInfo += preInit;
                    }
                    else
                    {
                        lastPhase.Content += preInit[..2];
                    }
                }

                preInit = null;
            }
            else
            {
                if (preInit.StartsWith("\r\nTrees after") && lastPhase is not null)
                {
                    var treesBeforeIndex = preInit.IndexOf("Trees before");
                    if (treesBeforeIndex == -1)
                    {
                        lastPhase.PostInfo = preInit[..(preInit.Length - 2)];
                        preInit = null;
                    }
                    else
                    {
                        lastPhase.PostInfo = preInit[0..(treesBeforeIndex - 2)];
                        preInit = preInit[treesBeforeIndex..];
                    }
                }
                else if (lastPhase is not null)
                {
                    preInit = preInit[2..];
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

    private void ParseBasicBlocks(PhaseInformation phase)
    {
        List<MethodBody> result = phase.MethodsDefinitions;
        if (phase.PreInfo is not null)
        {
            result.AddRange(ParseBasicBlocks(phase.PreInfo));
        }

        result.AddRange(ParseBasicBlocks(phase.Content));
        if (phase.PostInfo is not null)
        {
            result.AddRange(ParseBasicBlocks(phase.PostInfo));
        }
    }

    public List<MethodBody> ParseBasicBlocks(string content)
    {
        List<MethodBody> result = new();
        int offset = 0;
        (MethodBody MethodBody, int offset)? parseResult;
        do
        {
            parseResult = ParseMethodBody(content, offset);
            if (parseResult.HasValue)
            {
                offset = parseResult.Value.offset;
                result.Add(parseResult.Value.MethodBody);
            }
        }
        while (parseResult.HasValue);
        return result;
    }

    public (MethodBody MethodBody, int offset)? ParseMethodBody(string content, int offset)
    {
        MethodBody methodBody = new();
        var tableLine = "-----------------------------------------------------------------------------------------------------------------------------------------";
        offset = content.IndexOf(tableLine, offset);
        if (offset == -1)
        {
            return null;
        }

        offset += tableLine.Length;
        offset = content.IndexOf(tableLine, offset);
        if (offset == -1)
        {
            return null;
        }

        offset += tableLine.Length;
        var tableEnd = content.IndexOf(tableLine, offset);
        if (tableEnd == -1)
        {
            return null;
        }

        var blockRows = content[offset..tableEnd];
        var s = new StringReader(blockRows);
        string? line = s.ReadLine();
        while (line is not null)
        {
            line = s.ReadLine();
            if (line is not null)
            {
                var basicBlock = ParseBasicBlock(line);
                methodBody.BasicBlocks.Add(basicBlock);
            }
        }

        return (methodBody, tableEnd + tableLine.Length);
    }

    public BasicBlock ParseBasicBlock(string content)
    {
        var bbNum = content[2..4];
        var bbId = content[6..10];
        var bbref = content[12..14];
        var bbpred = content[23..25];
        var ilOffsetStart = content[52..55];
        var ilOffsetEnd = content[57..60];
        var flags = content[98..];

        var block = new BasicBlock()
        {
            SourceString = content,
        };
        return block;
    }
}
