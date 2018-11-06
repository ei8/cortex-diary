using NLog;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public NeuronClient(ISettingsService settingsService = null)
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public async Task AddTerminalsToNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.AddTerminalsToNeuronInternal(avatarUrl, id, terminals, expectedVersion, token).ConfigureAwait(false));

        public async Task AddTerminalsToNeuronInternal(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
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

        public async Task CreateNeuron(string avatarUrl, string id, string data, string authorId, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateNeuronInternal(avatarUrl, id, data, authorId, terminals, token).ConfigureAwait(false));

        private async Task CreateNeuronInternal(string avatarUrl, string id, string data, string authorId, IEnumerable<Terminal> terminals, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
            };

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"Data\": \"");
            sb.Append(data);
            sb.Append("\", ");
            sb.Append("\"AuthorId\": \"");
            sb.Append(authorId);
            sb.Append("\"");
            if (terminals.Any())
            {
                bool isFirst = true;
                sb.Append(", \"Terminals\": [");
                terminals.ToList().ForEach(t => {
                    if (!isFirst)
                        sb.Append(",");
                    sb.Append("{")
                        .Append($"\"TargetId\": \"{t.TargetId}\",")
                        .Append($"\"Effect\": \"{t.Effect.ToString()}\",")
                        .Append($"\"Strength\": \"{t.Strength.ToString()}\"");
                    sb.Append("}");
                    isFirst = false;
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

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }

        public async Task ChangeNeuronData(string avatarUrl, string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.ChangeNeuronDataInternal(avatarUrl, id, data, expectedVersion, token).ConfigureAwait(false));

        private async Task ChangeNeuronDataInternal(string avatarUrl, string id, string data, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
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

        public async Task RemoveTerminalsFromNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.RemoveTerminalsFromNeuronInternal(avatarUrl, id, terminals, expectedVersion, token));

        public async Task RemoveTerminalsFromNeuronInternal(string avatarUrl, string id, IEnumerable<Terminal> terminals, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
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

        public async Task DeactivateNeuron(string avatarUrl, string id, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
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
