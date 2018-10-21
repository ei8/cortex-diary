using DynamicData.Binding;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class PropertyGridViewModel : ToolViewModel
    {
        private IExtendedSelectionService selectionService;

        public PropertyGridViewModel(IExtendedSelectionService selectionService = null) : base("Properties")
        {
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>();

            this.selectionService.WhenPropertyChanged(a => a.SelectedComponents)
                .Subscribe(p =>
                {
                    this.SelectedObject = p.Sender.PrimarySelection;
                    }
                );
        }

        private object selectedObject;

        public object SelectedObject
        {
            get => this.selectedObject;
            set => this.RaiseAndSetIfChanged(ref this.selectedObject, value);
        }
    }
}
