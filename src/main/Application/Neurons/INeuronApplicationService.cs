using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public interface INeuronApplicationService
    {
        Task AddTerminalsToNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken));
        Task ChangeNeuronData(string avatarUrl, string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken));
        Task CreateNeuron(string avatarUrl, string id, string data, string authorId, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken));
        Task RemoveTerminalsFromNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken));
        Task DeactivateNeuron(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
