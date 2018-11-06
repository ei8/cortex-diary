using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public interface IStatusService : INotifyPropertyChanged
    {
        string Message
        {
            get; set;
        }
    }
}
