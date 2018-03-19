using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public interface INeuronClient
    {
        Task AddTerminalsToNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task CreateNeuron(string id, string data, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken));

        Task ChangeNeuronData(string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task RemoveTerminalsFromNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task DeactivateNeuron(string id, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
