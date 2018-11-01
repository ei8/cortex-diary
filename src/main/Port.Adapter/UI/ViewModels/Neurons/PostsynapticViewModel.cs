using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class PostsynapticViewModel : NeuronViewModelBase
    {
        public PostsynapticViewModel(string data, Node<Neuron, int> node, SourceCache<Neuron, int> cache, NeuronViewModelBase parent = null, INeuronService neuronService = null, INeuronQueryService neuronQueryService = null, IExtendedSelectionService selectionService = null) : base(node, cache, parent, neuronService, neuronQueryService, selectionService)
        {
            this.Data = data;
        }

        public override object ViewModel => this;
    }
}
