using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public interface ITerminalClient
    {
        Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, CancellationToken token = default(CancellationToken));
        Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
