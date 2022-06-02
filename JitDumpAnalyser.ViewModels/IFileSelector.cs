namespace JitDumpAnalyser.ViewModels;

public interface IFileSelector
{
    Task<string> SelectFileAsync();
}
