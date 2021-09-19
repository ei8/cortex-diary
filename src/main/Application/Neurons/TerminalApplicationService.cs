using neurUL.Cortex.Common;
using Splat;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Diary.Domain.Model;

namespace ei8.Cortex.Diary.Application.Neurons
{
    public class TerminalApplicationService : ITerminalApplicationService
    {
        private ITerminalClient terminalClient;
        private ITokenManager tokenManager;

        public TerminalApplicationService(ITerminalClient terminalClient = null, ITokenManager tokenManager = null)
        {
            this.terminalClient = terminalClient ?? Locator.Current.GetService<ITerminalClient>();
            this.tokenManager = tokenManager ?? Locator.Current.GetService<ITokenManager>();
        }

        public async Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string url, CancellationToken token = default(CancellationToken))
        {
            await this.terminalClient.CreateTerminal(avatarUrl, id, presynapticNeuronId, postsynapticNeuronId, effect, strength, url, await this.tokenManager.RetrieveAccessTokenAsync(), token);
        }

        public async Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.terminalClient.DeactivateTerminal(avatarUrl, id, expectedVersion, await this.tokenManager.RetrieveAccessTokenAsync(), token);
        }
    }
}
