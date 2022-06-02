using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using JitDumpAnalyser.Core;

namespace JitDumpAnalyser.ViewModels;

public class MainViewModel: INotifyPropertyChanged
{
    private IFileSelector fileSelector;

    public MainViewModel(IFileSelector fileSelector)
    {
        this.fileSelector = fileSelector;
    }

    public ICommand LoadDump => new RelayCommand(LoadTarget);

    public DumpParserResult? ParserResult { get; internal set; }

    public ObservableCollection<MethodCompilationModel> ParsedMethods { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private async void LoadTarget(object? parameter)
    {
        var file = await fileSelector.SelectFileAsync();
        if (file == null)
        {
            return;
        }

        var content = await File.ReadAllTextAsync(file);
        var parser = new DumpParser();
        var dumpResult = parser.Parse(content);
        this.ParserResult = dumpResult;
        this.ParsedMethods.Clear();
        foreach (var item in dumpResult.ParsedMethods)
        {
            ParsedMethods.Add(new MethodCompilationModel(item));
        }
    }

    private void NotifyPropertyChanged([CallerMemberName]string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
