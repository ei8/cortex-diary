using ReactiveUI;
using System;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public class StatusService : ReactiveObject, IStatusService
    {
        private string message;

        public string Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref this.message, value);
        }
    }
}
