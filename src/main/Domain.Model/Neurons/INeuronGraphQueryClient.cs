using DynamicData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public interface INeuronGraphQueryClient
    {
        Task<IEnumerable<Neuron>> GetAll(Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = default(string), int? limit = 1000, CancellationToken token = default(CancellationToken));

        Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken));
    }
}
