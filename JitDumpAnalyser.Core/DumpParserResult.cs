using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitDumpAnalyser.Core
{
    public class DumpParserResult
    {
        public List<MethodCompilationResult> ParsedMethods { get; } = new();
    }
}
