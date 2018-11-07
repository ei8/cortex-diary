using System.Windows;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public class DialogService : IDialogService
    {
        public DialogResult ShowDialogYesNo(string message, object owner)
        {
            return this.ShowDialog<DialogResult>(new DialogYesNoViewModel(message), owner);
        }

        private T ShowDialog<T>(DialogViewModelBase vm, object owner)
        {
            DialogWindow win = new DialogWindow();
            if (owner != null)
                win.Owner = owner as Window;
            win.DataContext = vm;
            win.ShowDialog();
            return (T) vm.UserDialogResult;
        }
    }
}
