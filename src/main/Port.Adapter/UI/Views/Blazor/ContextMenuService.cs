using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public class ContextMenuService : IContextMenuService
    {
        private object selection;
        public object Selection => this.selection;

        public event Action MenuRequested;

        public void Request(object selection)
        {
            this.selection = selection;
            this.MenuRequested?.Invoke();
        }
    }
}
