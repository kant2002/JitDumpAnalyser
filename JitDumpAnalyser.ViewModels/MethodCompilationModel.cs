using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JitDumpAnalyser.Core;

namespace JitDumpAnalyser.ViewModels;

public partial class MethodCompilationModel : ObservableObject
{
    private readonly MethodCompilationResult method;
    [ObservableProperty]
    private PhaseInformation? selectedPhase;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Phases))]
    private bool showOnlyBasicBlocks;

    public MethodCompilationModel(MethodCompilationResult method)
    {
        this.method = method;
    }

    public string MethodName => method.MethodName;

    public string Content => method.Content;

    public uint MethodHash => method.MethodHash;

    public List<PhaseInformation> Phases => method.Phases.Where(_ => !ShowOnlyBasicBlocks || _.MethodsDefinitions.Count > 0).ToList();

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
}
