﻿using DynamicData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public interface INeuronQueryService
    {
        Task<IEnumerable<Neuron>> GetAll(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = default(string), int? limit = 1000, CancellationToken token = default(CancellationToken));

        Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken));
    }
}
