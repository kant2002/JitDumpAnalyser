using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JitDumpAnalyser.Core
{
    public class MethodCompilationResult
    {
        public MethodCompilationResult(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; internal init; }

        public string Content { get; internal init; }

        public uint MethodHash { get; set; }
        public List<PhaseInformation> Phases { get; } = new();
    }
}
