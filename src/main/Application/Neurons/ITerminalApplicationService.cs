using neurUL.Cortex.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Neurons
{
    public interface ITerminalApplicationService
    {
        Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string bearerToken, CancellationToken token = default(CancellationToken));

        Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, string bearerToken, CancellationToken token = default(CancellationToken));
    }
}
