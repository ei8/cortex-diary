using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels
{
    public class SelectionService : ReactiveObject, IExtendedSelectionService
    {
        private ICollection selectedComponents;

        public object PrimarySelection => this.selectedComponents != null ? this.selectedComponents.OfType<object>().FirstOrDefault() : null;

        public int SelectionCount => this.selectedComponents != null ? this.selectedComponents.Count : 0;

        public event EventHandler SelectionChanged;
        public event EventHandler SelectionChanging;

        public SelectionService()
        {

        }

        public bool GetComponentSelected(object component)
        {
            throw new NotImplementedException();
        }

        public ICollection GetSelectedComponents()
        {
            return this.SelectedComponents;
        }

        public void SetSelectedComponents(ICollection components)
        {
            this.SelectedComponents = components;
        }

        public void SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            throw new NotImplementedException();
        }

        public ICollection SelectedComponents
        {
            get => this.selectedComponents;
            set => this.RaiseAndSetIfChanged(ref this.selectedComponents, value);
        }
    }
}
