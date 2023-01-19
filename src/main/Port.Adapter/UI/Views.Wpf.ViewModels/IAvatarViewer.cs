using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public interface IAvatarViewer : INotifyPropertyChanged
    {
        string AvatarUrl { get; }
        string RegionId { get; }
        string RegionName { get; }
        bool Loading { get; set; }
        EditorNeuronData Target { get; set; }
    }
}
