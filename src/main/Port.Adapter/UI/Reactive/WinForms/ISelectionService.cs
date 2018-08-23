using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public interface ISelectionService
    {
        event EventHandler SelectionChanged;

        IEnumerable<object> SelectedObjects { get; }

        void SetSelectedObjects(IEnumerable<object> selection);
    }
}
