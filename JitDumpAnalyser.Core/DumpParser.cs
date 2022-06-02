namespace JitDumpAnalyser.Core
{
    public class DumpParser
    {
        private const string StartMarker = "****** START compiling ";
        private const string EndMarker = "****** DONE compiling ";
        private const string MethodHashMarker = " (MethodHash=";

        public DumpParserResult Parse(string content)
        {
            var seekIndex = 0;
            var result = new DumpParserResult();
            while (seekIndex < content.Length)
            {
                var startIndex = content.IndexOf(StartMarker, seekIndex);
                if (startIndex < 0)
                {
                    return result;
                }

                var endIndex = content.IndexOf(EndMarker, startIndex + StartMarker.Length);
                var spaceMarker = content.IndexOf(' ', startIndex + StartMarker.Length);
                var methodName = content.Substring(startIndex + StartMarker.Length, spaceMarker - (startIndex + StartMarker.Length));
                var hashIndex = spaceMarker;
                var hashEndIndex = content.IndexOf(")", spaceMarker + MethodHashMarker.Length);
                var methodHash = uint.Parse(content.Substring(spaceMarker + MethodHashMarker.Length, hashEndIndex - hashIndex - MethodHashMarker.Length), System.Globalization.NumberStyles.HexNumber);
                var methodCompilationResult = new MethodCompilationResult(methodName)
                {
                    MethodHash = methodHash,
                    Content = content.Substring(startIndex, endIndex - startIndex + methodName.Length + EndMarker.Length),
                };
                result.ParsedMethods.Add(methodCompilationResult);
                seekIndex = endIndex + EndMarker.Length + methodName.Length ;
            }

            return result;
        }
    }
}