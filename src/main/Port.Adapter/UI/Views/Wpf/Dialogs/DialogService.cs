using System.Collections.Generic;
using System.Threading.Tasks;
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
        public Task<bool?> ShowDialogSelectNeurons(string message, string avatarUrl, object owner, bool allowMultiSelect, out IEnumerable<Neuron> result)
        {
            return this.ShowDialog<IEnumerable<Neuron>>(new DialogSelectNeuronsViewModel(message, avatarUrl, allowMultiSelect), owner, out result);
        }

        public Task<bool?> ShowDialogYesNo(string message, object owner, out DialogResult result)
        {
            return this.ShowDialog<DialogResult>(new DialogYesNoViewModel(message), owner, out result);
        }

        public Task<bool?> ShowDialogTextInput(string message, string avatarUrl, object owner, out string result)
        {
            return this.ShowDialog<string>(new DialogTextInputViewModel(message), owner, out result);
        }

        private Task<bool?> ShowDialog<T>(DialogViewModelBase vm, object owner, out T result)
        {
            DialogWindow win = new DialogWindow();
            if (owner != null)
                win.Owner = owner as Window;
            win.DataContext = vm;
            win.ShowDialog();
            result = (T) vm.UserDialogResult;
            return Task.FromResult(vm.DialogResult);
        }
    }
}
