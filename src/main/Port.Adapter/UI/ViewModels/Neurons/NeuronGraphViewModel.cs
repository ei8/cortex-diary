using DynamicData.Binding;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class NeuronGraphViewModel : ToolViewModel
    {
        private IExtendedSelectionService selectionService;

        public NeuronGraphViewModel(IExtendedSelectionService selectionService = null) : base("Graph")
        {
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>();

            this.selectionService.WhenPropertyChanged(a => a.SelectedComponents)
                .Subscribe(p =>
                {
                    if (p.Sender.PrimarySelection is NeuronViewModelBase)
                        this.SelectedNeuron = (NeuronViewModelBase)p.Sender.PrimarySelection;
                });
        }

        private NeuronViewModelBase selectedNeuron;

        public NeuronViewModelBase SelectedNeuron
        {
            get => this.selectedNeuron;
            set => this.RaiseAndSetIfChanged(ref this.selectedNeuron, value);
        }
    }
}
