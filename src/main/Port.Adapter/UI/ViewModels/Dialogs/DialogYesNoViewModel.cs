using ReactiveUI;
using System.Windows.Input;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public class DialogYesNoViewModel : DialogViewModelBase
    {
        public ReactiveCommand YesCommand { get; }

        public ReactiveCommand NoCommand { get; }

        private bool closing;

        public bool Closing
        {
            get => this.closing;
            set => this.RaiseAndSetIfChanged(ref this.closing, value);
        }

        public DialogYesNoViewModel(string message)
            : base(message)
        {
            this.YesCommand = ReactiveCommand.Create(OnYesClicked);
            this.NoCommand = ReactiveCommand.Create(OnNoClicked);
        }

        private void OnYesClicked()
        {
            this.UserDialogResult = Dialogs.DialogResult.Yes;
            this.DialogResult = true;
            this.Closing = true;
        }

        private void OnNoClicked()
        {
            this.UserDialogResult = Dialogs.DialogResult.No;
            this.DialogResult = true;
            this.Closing = true;
        }
    }
}
