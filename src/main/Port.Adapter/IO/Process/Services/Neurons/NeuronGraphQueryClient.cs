using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using org.neurul.Common;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public NeuronGraphQueryClient(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public async Task<Neuron> GetNeuron(string id, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.GetNeuronInternal(id, token).ConfigureAwait(false));

        private async Task<Neuron> GetNeuronInternal(string id, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };
            
            // neuron
            var response = await httpClient.GetAsync(
                string.Format(NeuronGraphQueryClient.neuronsQueryPathTemplate, id)
                ).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject jd = JsonHelper.JObjectParse(content);
            Neuron result = new Neuron()
            {
                Id = JsonHelper.GetRequiredValue<string>(jd, "Id"),
                Data = JsonHelper.GetRequiredValue<string>(jd, "Data"),
                Version = JsonHelper.GetRequiredValue<int>(jd, "Version"),
                Timestamp = JsonHelper.GetRequiredValue<string>(jd, "Timestamp"),
            };

            var tlist = new List<Terminal>();
            foreach (JToken to in JsonHelper.GetRequiredChildren(jd, "Terminals"))
                tlist.Add(
                    new Terminal()
                    {
                        TargetId = JsonHelper.GetRequiredValue<string>(to, "TargetId"),
                        TargetData = JsonHelper.GetRequiredValue<string>(to, "TargetData")
                    }
                    );
            result.Axon = tlist.ToArray();

            // dendrites
            response = await httpClient.GetAsync(
                string.Format(NeuronGraphQueryClient.dendritesQueryPathTemplate, id)
                ).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jds = NeuronGraphQueryClient.ParseArray(content);
            var plist = new List<Dendrite>();
            foreach(JObject jo in jds)
            {
                plist.Add(
                        new Dendrite(
                            JsonHelper.GetRequiredValue<string>(jo, "Id"),
                            JsonHelper.GetRequiredValue<string>(jo, "Data"),
                            JsonHelper.GetRequiredValue<int>(jo, "Version")
                        )
                    );
            }
            result.Dendrites = plist;

            return result;
        }

        // TODO: transfer to common
        private static JObject[] ParseArray(string value)
        {
            JArray a = JArray.Parse(value);

            return a.Children<JObject>().ToArray();
        }

        public async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstring(string dataSubstring, CancellationToken token = default(CancellationToken)) =>
            await NeuronGraphQueryClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.GetAllNeuronsByDataSubstringInternal(dataSubstring, token).ConfigureAwait(false));

        private async Task<IEnumerable<Neuron>> GetAllNeuronsByDataSubstringInternal(string dataSubstring, CancellationToken token = default(CancellationToken))
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.settingsService.AvatarEndpoint)
            };

            // neuron
            var response = await httpClient.GetAsync(
                string.Format(NeuronGraphQueryClient.neuronsQuerySearchPathTemplate, dataSubstring)
                ).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var jds = NeuronGraphQueryClient.ParseArray(content);
            var nlist = new List<Neuron>();
            foreach (JObject jo in jds)
            {
                var n = new Neuron()
                {
                    Id = JsonHelper.GetRequiredValue<string>(jo, "Id"),
                    Data = JsonHelper.GetRequiredValue<string>(jo, "Data"),
                    Version = JsonHelper.GetRequiredValue<int>(jo, "Version"),
                    Timestamp = JsonHelper.GetRequiredValue<string>(jo, "Timestamp"),
                };

                var tlist = new List<Terminal>();
                foreach (JToken to in JsonHelper.GetRequiredChildren(jo, "Terminals"))
                    tlist.Add(
                        new Terminal()
                        {
                            TargetId = JsonHelper.GetRequiredValue<string>(to, "TargetId"),
                            TargetData = JsonHelper.GetRequiredValue<string>(to, "TargetData")
                        }
                        );
                n.Axon = tlist.ToArray();

                nlist.Add(n);
            }

            return nlist;
        }
    }
}
