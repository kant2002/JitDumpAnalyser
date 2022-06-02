using System.Threading.Tasks;

namespace JitDumpAnalyser;

internal interface IFileSelector
{
    Task<string> SelectFileAsync();
}
