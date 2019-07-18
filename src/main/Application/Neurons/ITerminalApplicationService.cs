using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public interface ITerminalApplicationService
    {
        Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken));

        Task DeactivateTerminal(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
