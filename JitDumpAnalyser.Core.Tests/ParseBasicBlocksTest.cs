namespace JitDumpAnalyser.Core.Tests;

public class ParseBasicBlocksTest
{
    [Fact]
    public void ParseBasicBlocks()
    {
        var content = """
            -----------------------------------------------------------------------------------------------------------------------------------------
            BBnum BBid ref try hnd preds           weight    lp [IL range]     [jump]      [EH region]         [flags]
            -----------------------------------------------------------------------------------------------------------------------------------------
            BB01 [0000]  1                             1       [???..???)                                     keep i internal label LIR 
            BB02 [0001]  1       BB01                  1       [000..00C)        (return)                     i hascall gcsafe LIR 
            -----------------------------------------------------------------------------------------------------------------------------------------
            """;

        var parser = new DumpParser();
        var parseResult = parser.ParseBasicBlocks(content);

        var methodInformation = Assert.Single(parseResult);
        Assert.Equal(2, methodInformation.BasicBlocks.Count);
        Assert.Equal("BB01 [0000]  1                             1       [???..???)                                     keep i internal label LIR ", methodInformation.BasicBlocks[0].ToString());
        Assert.Equal("BB02 [0001]  1       BB01                  1       [000..00C)        (return)                     i hascall gcsafe LIR ", methodInformation.BasicBlocks[1].ToString());
    }
}
