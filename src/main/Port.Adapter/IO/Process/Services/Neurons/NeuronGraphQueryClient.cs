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

        private static readonly string GetNeuronsPathTemplate = "cortex/graph/neurons";
        private static readonly string GetRelativesPathTemplate = "cortex/graph/neurons/{0}/relatives";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NeuronGraphQueryClient(IRequestProvider requestProvider = null, ISettingsService settingsService = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public async Task<IEnumerable<Neuron>> GetNeuronById(string avatarUrl, string id, Neuron central = null, RelativeType type = RelativeType.NotSet, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetNeuronByIdInternal(avatarUrl, id, central, type, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetNeuronByIdInternal(string avatarUrl, string id, Neuron central = null, RelativeType type = RelativeType.NotSet, CancellationToken token = default(CancellationToken))
        {
            string path = string.Empty;
            var queryStringBuilder = new StringBuilder();

            if (central != null && type != RelativeType.NotSet)
            {
                path = $"{NeuronGraphQueryClient.GetNeuronsPathTemplate}/{central.NeuronId}/relatives/{id}";
                queryStringBuilder.Append($"?{nameof(type)}={type.ToString()}");
            }
            else
                path = $"{NeuronGraphQueryClient.GetNeuronsPathTemplate}/{id}";

            return await this.requestProvider.GetAsync<IEnumerable<Neuron>>(
               $"{avatarUrl}{path}{queryStringBuilder.ToString()}",
               this.settingsService.AuthAccessToken
               );
        }

        public async Task<IEnumerable<Neuron>> GetNeurons(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = null, int? limit = 1000, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetNeuronsInternal(avatarUrl, central, type, filter, limit, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetNeuronsInternal(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            var queryStringBuilder = new StringBuilder();

            // TODO: if (type != RelativeType.NotSet)
            //    queryStringBuilder.Append("type=")
            //        .Append(type.ToString());
            //if (!string.IsNullOrEmpty(filter))
            //    queryStringBuilder.Append("filter=")
            //        .Append(filter);
            if (limit.HasValue)
                queryStringBuilder.Append("limit=")
                    .Append(limit.Value);
            if (queryStringBuilder.Length > 0)
                queryStringBuilder.Insert(0, '?');

            var path = central == null ? NeuronGraphQueryClient.GetNeuronsPathTemplate : string.Format(NeuronGraphQueryClient.GetRelativesPathTemplate, central.NeuronId);

            return await this.requestProvider.GetAsync<IEnumerable<Neuron>>(
                $"{avatarUrl}{path}{queryStringBuilder.ToString()}",
                this.settingsService.AuthAccessToken
                );
        }
    }
}
