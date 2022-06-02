using JitDumpAnalyser.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JitDumpAnalyser;

internal class MethodCompilationModel : INotifyPropertyChanged
{
    private readonly MethodCompilationResult method;
    private PhaseInformation selectedPhase;

    public MethodCompilationModel(MethodCompilationResult method)
    {
        this.method = method;
    }

    public string MethodName => method.MethodName;

    public string Content => method.Content;

    public uint MethodHash => method.MethodHash;
    public List<PhaseInformation> Phases => method.Phases;

    public PhaseInformation SelectedPhase
    {
        get => selectedPhase;
        set
        {
            selectedPhase = value;
            this.NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
