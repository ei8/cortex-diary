using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public interface IDialogService
    {
        bool? ShowDialogYesNo(string message, object owner, out DialogResult result);

        bool? ShowDialogSelectNeuron(string message, string avatarUrl, object owner, out Neuron result);
    }
}
