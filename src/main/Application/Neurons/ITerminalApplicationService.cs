using neurUL.Cortex.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Neurons
{
    public interface ITerminalApplicationService
    {
        Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, CancellationToken token = default(CancellationToken));

        Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
