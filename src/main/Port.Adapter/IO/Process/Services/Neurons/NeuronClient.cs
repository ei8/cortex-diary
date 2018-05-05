using NLog;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class NeuronClient : INeuronClient
    {
        private readonly ISettingsService settingsService;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => NeuronClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static string neuronsPathTemplate = "cortex/neurons/{0}";
        private static string neuronsTerminalsPathTemplate = NeuronClient.neuronsPathTemplate + "/terminals";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NeuronClient(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public async Task AddTerminalsToNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.AddTerminalsToNeuronInternal(id, terminals, expectedVersion, token).ConfigureAwait(false));

        public async Task AddTerminalsToNeuronInternal(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if(terminals.Any())
            {
                bool isFirst = true;
                sb.Append(" \"Terminals\": [");
                terminals.ToList().ForEach(t => {
                    if (!isFirst)
                        sb.Append(",");
                    sb.Append("{ \"Target\": \"");
                    sb.Append(t.TargetId);
                    sb.Append("\"}");
                });
                sb.Append("]");
            }
            sb.Append("}");

            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(string.Format(NeuronClient.neuronsTerminalsPathTemplate, id), UriKind.Relative)
            };
            msg.Headers.Add("ETag", expectedVersion.ToString());
            msg.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(msg, token);
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateNeuron(string id, string data, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateNeuronInternal(id, data, terminals, token).ConfigureAwait(false));

        private async Task CreateNeuronInternal(string id, string data, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"Data\": \"");
            sb.Append(data);
            sb.Append("\"");
            if (terminals.Any())
            {
                bool isFirst = true;
                sb.Append(", \"Terminals\": [");
                terminals.ToList().ForEach(t => {
                    if (!isFirst)
                        sb.Append(",");
                    sb.Append("{ \"Target\": \"");
                    sb.Append(t.TargetId);
                    sb.Append("\"}");
                    });
                sb.Append("]");
            }
            sb.Append("}");
            
            StringContent content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(
                string.Format(NeuronClient.neuronsPathTemplate, id),
                content,
                token
                );
            response.EnsureSuccessStatusCode();
        }

        public async Task ChangeNeuronData(string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.ChangeNeuronDataInternal(id, data, expectedVersion, token).ConfigureAwait(false));

        private async Task ChangeNeuronDataInternal(string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"Data\": \"");
            sb.Append(data);
            sb.Append("\"");            
            sb.Append("}");

            HttpRequestMessage msg = new HttpRequestMessage {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(string.Format(NeuronClient.neuronsPathTemplate, id), UriKind.Relative)
            };
            msg.Headers.Add("ETag", expectedVersion.ToString());
            msg.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(msg, token);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveTerminalsFromNeuron(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.RemoveTerminalsFromNeuronInternal(id, terminals, expectedVersion, token));

        public async Task RemoveTerminalsFromNeuronInternal(string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = new HttpMethod("DELETE"),
                RequestUri = new Uri(string.Format(NeuronClient.neuronsTerminalsPathTemplate + "/{1}", id, terminals.First().TargetId), UriKind.Relative)
            };
            msg.Headers.Add("ETag", expectedVersion.ToString());

            HttpResponseMessage response = await httpClient.SendAsync(msg, token);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeactivateNeuron(string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = new HttpMethod("DELETE"),
                RequestUri = new Uri(string.Format(NeuronClient.neuronsPathTemplate, id), UriKind.Relative)
            };
            msg.Headers.Add("ETag", expectedVersion.ToString());

            HttpResponseMessage response = await httpClient.SendAsync(msg, token);
            response.EnsureSuccessStatusCode();
        }
    }
}
