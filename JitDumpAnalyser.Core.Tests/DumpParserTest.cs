namespace JitDumpAnalyser.Core.Tests
{
    public class DumpParserTest
    {
        [Fact]
        public void FindMethods()
        {
            var content = File.ReadAllText("Dump1.txt");

            var parser = new DumpParser();
            var parseResult = parser.Parse(content);

            Assert.Equal(2, parseResult.ParsedMethods.Count);
            Assert.Equal("System.Console:WriteLine(System.String)", parseResult.ParsedMethods[0].MethodName);
            Assert.Equal(0x450bf92fu, parseResult.ParsedMethods[0].MethodHash);
            Assert.Equal("System.Console:WriteLine(System.Object)", parseResult.ParsedMethods[1].MethodName);
            Assert.Equal(0x8fbe370fu, parseResult.ParsedMethods[1].MethodHash);
            Assert.NotNull(parseResult);
        }

        [Fact]
        public void FindPhases()
        {
            var content = File.ReadAllText("Dump1.txt");

            var parser = new DumpParser();
            var parseResult = parser.Parse(content);

            var methodResult = parseResult.ParsedMethods[0];
            Assert.Equal(37, methodResult.Phases.Count);
            var wellKnownPhases = new[]
            {
                "Pre-import",
                "Profile incorporation",
                "Importation",
                "Expand patchpoints",
                "Indirect call transform",
                "Post-import",
                "Morph - Init",
                "Morph - Inlining",
                "Allocate Objects",
                "Morph - Add internal blocks",
                "Remove empty try",
                "Remove empty finally",
                "Merge callfinally chains",
                "Clone finally",
                "Compute preds",
                "Morph - Promote Structs",
                "Morph - Structs/AddrExp",
                "Forward Substitution",
                "Morph - ByRefs",
                "Morph - Global",
                "GS Cookie",
                "Compute edge weights (1, false)",
                "Create EH funclets",
                "Mark local vars",
                "Find oper order",
                "Set block order",
                "Insert GC Polls",
                "Determine first cold block",
                "Rationalize IR", // Has some data before and after
                "Do 'simple' lowering",
                "Lowering nodeinfo",
                "Calculate stack level slots",
                "Linear scan register alloc",
                "Place 'align' instructions",
                "Generate code",
                "Emit code",
                "Emit GC+EH tables",
            };
            Assert.Equal(wellKnownPhases, methodResult.Phases.Select(phase => phase.Name));
            var profileIncorporationPhase = methodResult.Phases.First(phase => phase.Name == "Profile incorporation");
            Assert.Equal(@"*************** Starting PHASE Profile incorporation
BBOPT not set

*************** Finishing PHASE Profile incorporation", profileIncorporationPhase.Content);
            Assert.True(profileIncorporationPhase.NoChanges);

            var preImportPhase = methodResult.Phases.First(phase => phase.Name == "Pre-import");
            Assert.Equal(@"*************** Starting PHASE Pre-import

*************** Finishing PHASE Pre-import", preImportPhase.Content);
            Assert.False(preImportPhase.NoChanges);
        }
    }
}