using DynamicData;
using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public interface INeuronService
    {
        void Add(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void Reload(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto = null);

        void AddPostsynaptic(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void AddPresynaptic(SourceCache<NeuronDto, int> cache, NeuronDto neuronDto);

        void ChangeData(SourceCache<NeuronDto, int> cache, NeuronDto dto, string value);

        void Delete(SourceCache<NeuronDto, int> cache, NeuronDto dto);        
    }
}
