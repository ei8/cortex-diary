using Spiker.Neurons;
using Spiker.SpikeResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms
{
    public partial class PropertyGridForm : DockContent
    {
        private ISelectionService selectionService;

        public PropertyGridForm(ISelectionService selectionService)
        {
            InitializeComponent();

            this.selectionService = selectionService;
            this.selectionService.SelectionChanged += this.SelectionService_SelectionChanged;
        }

        private void SelectionService_SelectionChanged(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObjects = this.selectionService.SelectedObjects.ToArray();
        }
    }
}
