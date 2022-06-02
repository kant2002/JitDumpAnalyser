using JitDumpAnalyser.ViewModels;
using Microsoft.UI.Xaml;

namespace JitDumpAnalyser;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        MainViewModel mainViewModel = new MainViewModel(new WinUIFileSelector(this));
        parent.DataContext = mainViewModel;
        this.Title = "JIT Dump Analyzer";
    }
}
