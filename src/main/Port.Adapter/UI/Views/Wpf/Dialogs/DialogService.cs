using System.Windows;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public class DialogService : IDialogService
    {
        public bool? ShowDialogSelectNeuron(string message, string avatarUrl, object owner, out Neuron result)
        {
            return this.ShowDialog<Neuron>(new DialogSelectNeuronViewModel(message, avatarUrl), owner, out result);
        }

        public bool? ShowDialogYesNo(string message, object owner, out DialogResult result)
        {
            return this.ShowDialog<DialogResult>(new DialogYesNoViewModel(message), owner, out result);
        }

        private bool? ShowDialog<T>(DialogViewModelBase vm, object owner, out T result)
        {
            DialogWindow win = new DialogWindow();
            if (owner != null)
                win.Owner = owner as Window;
            win.DataContext = vm;
            win.ShowDialog();
            result = (T) vm.UserDialogResult;
            return vm.DialogResult;
        }
    }
}
