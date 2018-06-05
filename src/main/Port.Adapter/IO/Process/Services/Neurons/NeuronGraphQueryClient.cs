using NLog;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class NeuronGraphQueryClient : INeuronGraphQueryClient
    {
        private readonly IRequestProvider requestProvider;
        private readonly ISettingsService settingsService;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => NeuronGraphQueryClient.logger.Error(ex, "Error occurred while querying Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static readonly string neuronsQueryPathTemplate = "cortex/graph/neurons/{0}";
        private static readonly string neuronsQuerySearchPathTemplate = "cortex/graph/neurons/search?data={0}";
        private static readonly string dendritesQueryPathTemplate = NeuronGraphQueryClient.neuronsQueryPathTemplate + "/dendrites";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NeuronGraphQueryClient(IRequestProvider requestProvider, ISettingsService settingsService)
        {
            this.requestProvider = requestProvider;
            this.settingsService = settingsService;
        }

        public async Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetNeuronInternal(id, token).ConfigureAwait(false));

        private async Task<Neuron> GetNeuronInternal(string id, CancellationToken token = default(CancellationToken))
        {
            return await this.requestProvider.GetAsync<Neuron>(
                this.settingsService.AvatarEndpoint + string.Format(NeuronGraphQueryClient.neuronsQueryPathTemplate, id),
                this.settingsService.AuthAccessToken
                );
        }

        public async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.GetAllNeuronsByDataSubstringInternal(dataSubstring, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstringInternal(string dataSubstring, CancellationToken token = default(CancellationToken))
        {
            return await this.requestProvider.GetAsync<Neuron[]>(
                this.settingsService.AvatarEndpoint + string.Format(NeuronGraphQueryClient.neuronsQuerySearchPathTemplate, dataSubstring),
                this.settingsService.AuthAccessToken
                );
        }
    }
}
