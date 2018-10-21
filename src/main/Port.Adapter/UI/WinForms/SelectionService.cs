using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms
{
    public class SelectionService : ISelectionService
    {
        public event EventHandler SelectionChanged;

        private IEnumerable<object> selectedObjects;

        public IEnumerable<object> SelectedObjects => this.selectedObjects;        

        public void SetSelectedObjects(IEnumerable<object> selection)
        {
            this.selectedObjects = selection;
            this.SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
