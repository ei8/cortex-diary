using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Origin;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class PresynapticViewModel : NeuronViewModelBase
    {
        public PresynapticViewModel(string avatarUrl, string data, Node<Neuron, int> node, SourceCache<Neuron, int> cache, NeuronViewModelBase parent = null, INeuronService neuronService = null, INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null, IExtendedSelectionService selectionService = null) : base(avatarUrl, node, cache, parent, neuronService, neuronApplicationService, neuronQueryService, originService, selectionService)
        {
            this.Data = data;
        }

        public override object ViewModel => this;
    }
}
