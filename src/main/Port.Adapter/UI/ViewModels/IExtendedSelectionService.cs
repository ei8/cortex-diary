using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public interface IExtendedSelectionService : ISelectionService, INotifyPropertyChanged
    {
        ICollection SelectedComponents
        {
            get;
            set;
        }
    }
}
