using CommunityToolkit.Mvvm.Input;
using JitDumpAnalyser.Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JitDumpAnalyser.ViewModels;

public partial class MethodCompilationModel : INotifyPropertyChanged
{
    private readonly MethodCompilationResult method;
    private PhaseInformation? selectedPhase;

    public MethodCompilationModel(MethodCompilationResult method)
    {
        this.method = method;
    }

    public string MethodName => method.MethodName;

    public string Content => method.Content;

    public uint MethodHash => method.MethodHash;

    public List<PhaseInformation> Phases => method.Phases;

    [RelayCommand]
    private void NextPhase()
    {
        if (SelectedPhase is null)
        {
            SelectedPhase = Phases.FirstOrDefault();
        }
        else
        {
            var index = Phases.IndexOf(SelectedPhase);
            SelectedPhase = Phases[(index + 1) % Phases.Count];
        }
    }

    [RelayCommand]
    private void PrevPhase()
    {
        if (SelectedPhase is null)
        {
            SelectedPhase = Phases.LastOrDefault();
        }
        else
        {
            var index = Phases.IndexOf(SelectedPhase);
            SelectedPhase = Phases[(index - 1) % Phases.Count];
        }
    }

    public PhaseInformation? SelectedPhase
    {
        get => selectedPhase;
        set
        {
            selectedPhase = value;
            this.NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
