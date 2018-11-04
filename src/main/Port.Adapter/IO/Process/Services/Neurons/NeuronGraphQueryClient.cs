using NLog;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
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

        private static readonly string GetAllPathTemplate = "cortex/graph/neurons";
        private static readonly string GetAllRelativesPathTemplate = "cortex/graph/neurons/{0}/relatives";

        private static readonly string neuronsQueryPathTemplate = "cortex/graph/neurons/{0}";
        private static readonly string neuronsQuerySearchPathTemplate = "cortex/graph/neurons/search?data={0}";
        private static readonly string dendritesQueryPathTemplate = NeuronGraphQueryClient.neuronsQueryPathTemplate + "/dendrites";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NeuronGraphQueryClient(IRequestProvider requestProvider = null, ISettingsService settingsService = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public async Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetNeuronInternal(id, token).ConfigureAwait(false));

        private async Task<Neuron> GetNeuronInternal(string id, CancellationToken token = default(CancellationToken))
        {
            return await Task.FromResult(new Neuron());

            // TODO: return await this.requestProvider.GetAsync<Neuron>(
            //    this.settingsService.AvatarEndpoint + string.Format(NeuronGraphQueryClient.neuronsQueryPathTemplate, id),
            //    this.settingsService.AuthAccessToken
            //    );
        }

        public async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.GetAllNeuronsByDataSubstringInternal(dataSubstring, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstringInternal(string dataSubstring, CancellationToken token = default(CancellationToken))
        {
            return await Task.FromResult(new Neuron[0]);
            // TODO: return await this.requestProvider.GetAsync<Neuron[]>(
            //    this.settingsService.AvatarEndpoint + string.Format(NeuronGraphQueryClient.neuronsQuerySearchPathTemplate, dataSubstring),
            //    this.settingsService.AuthAccessToken
            //    );
        }

        public async Task<IEnumerable<Neuron>> GetAll(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = null, int? limit = 1000, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetAllInternal(avatarUrl, central, type, filter, limit, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetAllInternal(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            var queryStringBuilder = new StringBuilder();

            if (type != RelativeType.NotSet)
                queryStringBuilder.Append("type=")
                    .Append(type.ToString());
            if (!string.IsNullOrEmpty(filter))
                queryStringBuilder.Append("filter=")
                    .Append(filter);
            if (limit.HasValue)
                queryStringBuilder.Append("limit=")
                    .Append(limit.Value);
            if (queryStringBuilder.Length > 0)
                queryStringBuilder.Insert(0, '?');

            var path = central == null ? NeuronGraphQueryClient.GetAllPathTemplate : string.Format(NeuronGraphQueryClient.GetAllRelativesPathTemplate, central.NeuronId);

            return await this.requestProvider.GetAsync<IEnumerable<Neuron>>(
                $"{avatarUrl}{path}{queryStringBuilder.ToString()}",
                this.settingsService.AuthAccessToken
                );
        }
    }
}
