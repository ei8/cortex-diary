using DynamicData;
using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons
{
    public interface INeuronService
    {
        void Reload(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void AddPostsynaptic(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void AddPresynaptic(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void ChangeData(SourceCache<NeuronDto, int> cache, NeuronDto dto, string value);
    }
}
