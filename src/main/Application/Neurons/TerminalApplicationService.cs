using neurUL.Cortex.Common;
using Splat;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Nucleus.Client.In;

namespace ei8.Cortex.Diary.Application.Neurons
{
    public class TerminalApplicationService : ITerminalApplicationService
    {
        private ITerminalClient terminalClient;

        public TerminalApplicationService(ITerminalClient terminalClient = null)
        {
            this.terminalClient = terminalClient ?? Locator.Current.GetService<ITerminalClient>();
        }

        public async Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string bearerToken, CancellationToken token = default(CancellationToken))
        {
            await this.terminalClient.CreateTerminal(avatarUrl, id, presynapticNeuronId, postsynapticNeuronId, effect, strength, bearerToken, token);
        }

        public async Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, string bearerToken, CancellationToken token = default(CancellationToken))
        {
            await this.terminalClient.DeactivateTerminal(avatarUrl, id, expectedVersion, bearerToken, token);
        }
    }
}
