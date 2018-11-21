using DynamicData;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Origin;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class PostsynapticViewModel : NeuronViewModelBase
    {
        public PostsynapticViewModel(string avatarUrl, string tag, Node<Neuron, int> node, SourceCache<Neuron, int> cache, NeuronViewModelBase parent = null, 
            INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null, 
            IExtendedSelectionService selectionService = null) : 
            base(avatarUrl, node, cache, parent, neuronApplicationService, neuronQueryService, originService, selectionService)
        {
            this.Tag = tag;
        }

        public override object ViewModel => this;
    }
}
