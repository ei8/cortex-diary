using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronService : INeuronService
    {
        public void Add(SourceCache<Neuron, int> cache, Neuron neuron)
        {
            // TODO: Update data source
            cache.AddOrUpdate(neuron);
        }

        public void AddPostsynaptic(SourceCache<Neuron, int> cache, Neuron central)
        {
            var postsyn = new Neuron {
                Id = Guid.NewGuid().GetHashCode(),
                CentralId = central.Id,
                NeuronId = Guid.NewGuid().ToString(),
                CentralNeuronId = central.CentralNeuronId,
                Data = "New Postsynaptic",
                Type = RelativeType.Postsynaptic
                };

            cache.AddOrUpdate(postsyn);
        }

        public void AddPresynaptic(SourceCache<Neuron, int> cache, Neuron central)
        {
            var presyn = new Neuron
            {
                Id = Guid.NewGuid().GetHashCode(),
                CentralId = central.Id,
                NeuronId = Guid.NewGuid().ToString(),
                CentralNeuronId = central.CentralNeuronId,
                Data = "New Presynaptic",
                Type = RelativeType.Presynaptic
            };

            cache.AddOrUpdate(presyn);
        }

        public void ChangeData(SourceCache<Neuron, int> cache, Neuron dto, string value)
        {
            // TODO: update data source
            // TODO: retrieve updated value and replace value in children
            var newValue = new Neuron
            {
                Id = dto.Id,
                CentralId = dto.CentralId,
                NeuronId = dto.NeuronId,
                CentralNeuronId = dto.CentralNeuronId,
                Data = value,
                Type = dto.Type
            };

            cache.AddOrUpdate(newValue);
        }

        public void Delete(SourceCache<Neuron, int> cache, Neuron dto)
        {
            // TODO: update data source
            // TODO: warn user
            // TODO: if dto has a parent then user is deleting a relationship only, otherwise a neuron is being deleted
            cache.Remove(dto);
        }
    }
}
