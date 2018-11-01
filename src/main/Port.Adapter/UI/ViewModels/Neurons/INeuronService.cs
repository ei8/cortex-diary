using DynamicData;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public interface INeuronService
    {
        void Add(SourceCache<Neuron, int> cache, Neuron Neuron);

        void AddPostsynaptic(SourceCache<Neuron, int> cache, Neuron Neuron);

        void AddPresynaptic(SourceCache<Neuron, int> cache, Neuron Neuron);

        void ChangeData(SourceCache<Neuron, int> cache, Neuron dto, string value);

        void Delete(SourceCache<Neuron, int> cache, Neuron dto);        
    }
}
