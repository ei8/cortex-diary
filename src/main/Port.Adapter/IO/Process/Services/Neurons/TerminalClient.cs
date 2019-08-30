using NLog;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class TerminalClient : ITerminalClient
    {
        private readonly IRequestProvider requestProvider;
        private readonly ISettingsService settingsService;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => TerminalClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string terminalsPath = "cortex/terminals/";
        private static readonly string terminalsPathTemplate = terminalsPath + "{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TerminalClient(IRequestProvider requestProvider = null, ISettingsService settingsService = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public async Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, CancellationToken token = default(CancellationToken)) =>
            await TerminalClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateTerminalInternal(avatarUrl, id, presynapticNeuronId, postsynapticNeuronId, effect, strength, token).ConfigureAwait(false));

        public async Task CreateTerminalInternal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Id = id,
                PresynapticNeuronId = presynapticNeuronId,
                PostsynapticNeuronId = postsynapticNeuronId,
                Effect = effect.ToString(),
                Strength = strength.ToString(),
            };

            await this.requestProvider.PostAsync(
               $"{avatarUrl}{TerminalClient.terminalsPath}",
               data,
               this.settingsService.AuthAccessToken
               );
        }
        
        public async Task DeactivateTerminal(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await TerminalClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.DeactivateTerminalInternal(avatarUrl, id, expectedVersion, token).ConfigureAwait(false));

        public async Task DeactivateTerminalInternal(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            await this.requestProvider.DeleteAsync<object>(
               $"{avatarUrl}{string.Format(TerminalClient.terminalsPathTemplate, id)}",
               null,
               this.settingsService.AuthAccessToken,
               token,
               new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
