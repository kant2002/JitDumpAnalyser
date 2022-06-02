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
    }
}