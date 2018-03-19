using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public class NeuronQueryService : INeuronQueryService
    {
        private INeuronGraphQueryClient neuronQueryClient;

        public NeuronQueryService(INeuronGraphQueryClient neuronQueryClient)
        {
            this.neuronQueryClient = neuronQueryClient;
        }

        public async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken))
        {
            return await this.neuronQueryClient.GetAllNeuronsByDataSubstring(dataSubstring, token);
        }

        public async Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken))
        {
            return await this.neuronQueryClient.GetNeuron(id);
        }
    }
}
