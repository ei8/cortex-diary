using DynamicData;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public interface INeuronService
    {
        void ChangeData(SourceCache<Neuron, int> cache, Neuron dto, string value);

        void Delete(SourceCache<Neuron, int> cache, Neuron dto);        
    }
}
