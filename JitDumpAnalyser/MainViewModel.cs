using JitDumpAnalyser.Core;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace JitDumpAnalyser;

internal class MainViewModel: INotifyPropertyChanged
{
    private IntPtr hwnd;

    public MainViewModel(IntPtr windowHandle)
    {
        this.hwnd = windowHandle;
    }

    public ICommand LoadDump => new RelayCommand(LoadTarget);

    public DumpParserResult? ParserResult { get; internal set; }

    public ObservableCollection<MethodCompilationModel> ParsedMethods { get; } = new();

    public event PropertyChangedEventHandler PropertyChanged;

    private async void LoadTarget(object parameter)
    {           
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.List,
        };

        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add(".log");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file == null)
        {
            return;
        }

        var content = await File.ReadAllTextAsync(file.Path);
        var parser = new DumpParser();
        var dumpResult = parser.Parse(content);
        this.ParserResult = dumpResult;
        this.ParsedMethods.Clear();
        foreach (var item in dumpResult.ParsedMethods)
        {
            ParsedMethods.Add(new MethodCompilationModel(item));
        }

        //this.NotifyPropertyChanged(nameof(ParserResult));
        //this.NotifyPropertyChanged(nameof(ParsedMethods));
    }

    private void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
