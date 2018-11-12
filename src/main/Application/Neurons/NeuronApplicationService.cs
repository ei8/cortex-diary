using Splat;
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

        public NeuronApplicationService(INeuronClient neuronClient = null)
        {
            this.neuronClient = neuronClient ?? Locator.Current.GetService<INeuronClient>();
        }

        public async Task AddTerminalsToNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.AddTerminalsToNeuron(avatarUrl, id, terminals, authorId, expectedVersion, token);
        }

        public async Task ChangeNeuronData(string avatarUrl, string id, string data, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.ChangeNeuronData(avatarUrl, id, data, authorId, expectedVersion, token);
        }

        public async Task CreateNeuron(string avatarUrl, string id, string data, IEnumerable<Terminal> terminals, string authorId, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.CreateNeuron(avatarUrl, id, data, terminals, authorId, token);
        }

        public async Task RemoveTerminalsFromNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.RemoveTerminalsFromNeuron(avatarUrl, id, terminals, expectedVersion, token);
        }

        public async Task DeactivateNeuron(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.neuronClient.DeactivateNeuron(avatarUrl, id, expectedVersion, token);
        }
    }
}
