using DynamicData.Binding;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class NeuronGraphViewModel : ToolViewModel
    {
        private IExtendedSelectionService selectionService;
        private IExtendedSelectionService highlightService;

        public NeuronGraphViewModel(IExtendedSelectionService selectionService = null, IExtendedSelectionService highlightService = null) : base("Graph")
        {
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Select.ToString());
            this.highlightService = highlightService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Highlight.ToString());

            this.SelectCommand = ReactiveCommand.Create(() => this.UpdateHighlightService());

            this.selectionService.WhenPropertyChanged(a => a.SelectedComponents)
                .Subscribe(p =>
                {
                    if (p.Sender.PrimarySelection is NeuronViewModelBase)
                    {
                        this.ExternallySelectedNeuron = (NeuronViewModelBase)p.Sender.PrimarySelection;
                        this.InternallySelectedNeuronId = null;
                        this.UpdateHighlightService();
                    }
                });
        }

        private void UpdateHighlightService()
        {
            this.highlightService.SetSelectedComponents(new object[] { this.InternallySelectedNeuronId });
        }

        public ReactiveCommand<Unit, Unit> SelectCommand { get; }

        private NeuronViewModelBase externallySelectedNeuron;

        public NeuronViewModelBase ExternallySelectedNeuron
        {
            get => this.externallySelectedNeuron;
            set => this.RaiseAndSetIfChanged(ref this.externallySelectedNeuron, value);
        }

        private string internallySelectedNeuronId;

        public string InternallySelectedNeuronId
        {
            get => this.internallySelectedNeuronId;
            set => this.RaiseAndSetIfChanged(ref this.internallySelectedNeuronId, value);
        }
    }
}
