using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public interface IAvatarViewer : INotifyPropertyChanged
    {
        string AvatarUrl { get; }
        string LayerId { get; }
        string LayerName { get; }
        bool Loading { get; set; }
        EditorNeuronData Target { get; set; }
    }
}
