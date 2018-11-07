using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    /// <summary>
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public interface IDialogService
    {
        DialogResult ShowDialogYesNo(string message, object owner);
    }
}
