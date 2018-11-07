using ReactiveUI;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public abstract class DialogViewModelBase : ReactiveObject
    {
        private object userDialogResult;

        public object UserDialogResult
        {
            get => userDialogResult;
            set => this.RaiseAndSetIfChanged(ref this.userDialogResult, value);
        }

        private bool dialogResult;

        public bool DialogResult
        {
            get => this.dialogResult;
            set => this.RaiseAndSetIfChanged(ref this.dialogResult, value);
        }

        public string Message
        {
            get;
            private set;
        }

        public DialogViewModelBase(string message)
        {
            this.Message = message;
        }
    }
}
