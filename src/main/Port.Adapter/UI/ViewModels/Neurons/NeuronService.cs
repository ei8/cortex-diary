﻿using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronService : INeuronService
    {
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