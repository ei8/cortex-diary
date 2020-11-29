using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public interface IContextMenuService
    {
        object Selection { get; }

        event Action MenuRequested;

        void Request(object selection);
    }
}
