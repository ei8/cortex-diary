using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public class NeuronApplicationService : INeuronApplicationService
    {
        private INeuronClient neuronClient;

        public NeuronApplicationService(INeuronClient neuronClient)
        {
            this.neuronClient = neuronClient;
        }

        public async Task AddTerminalsToNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.AddTerminalsToNeuron(id, terminals, expectedVersion, token);
        }

        public async Task ChangeNeuronData(string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.ChangeNeuronData(id, data, expectedVersion, token);
        }

        public async Task CreateNeuron(string id, string data, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.CreateNeuron(id, data, terminals, token);
        }

        public async Task RemoveTerminalsFromNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.RemoveTerminalsFromNeuron(id, terminals, expectedVersion, token);
        }

        public async Task DeactivateNeuron(string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.DeactivateNeuron(id, expectedVersion, token);
        }
    }
}
