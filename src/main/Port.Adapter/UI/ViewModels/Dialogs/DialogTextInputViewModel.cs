using ReactiveUI;
using System.Windows.Input;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    public class DialogTextInputViewModel : DialogViewModelBase
    {
        public ReactiveCommand OkCommand { get; }

        public ReactiveCommand CancelCommand { get; }

        public DialogTextInputViewModel(string message)
            : base(message)
        {
            this.UserDialogResult = string.Empty;
            this.OkCommand = ReactiveCommand.Create(OnOkClicked);
            this.CancelCommand = ReactiveCommand.Create(OnCancelClicked);
        }

        private string textInput;

        public string TextInput
        {
            get => this.textInput;
            set => this.RaiseAndSetIfChanged(ref this.textInput, value); 
        }

        private void OnOkClicked()
        {
            this.UserDialogResult = this.textInput;
            this.DialogResult = true;
        }

        private void OnCancelClicked()
        {
            this.UserDialogResult = this.textInput;
            this.DialogResult = false;
        }
    }
}
