using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public interface IDialogService
    {
        Task<bool?> ShowDialogYesNo(string message, object owner, out DialogResult result);

        Task<bool?> ShowDialogSelectNeuron(string message, string avatarUrl, object owner, out Neuron result);

        Task<bool?> ShowDialogTextInput(string message, string avatarUrl, object owner, out string result);
    }
}
