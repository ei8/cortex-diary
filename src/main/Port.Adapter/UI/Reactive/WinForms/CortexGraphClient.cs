using Newtonsoft.Json.Linq;
using NLog;
using Polly;
using Spiker.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public class CortexGraphClient : ICortexGraphClient
    {
        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => CortexGraphClient.logger.Error(ex, "Error occured while subscribing to events. " + ex.InnerException?.Message)
            );
        private static HttpClient httpClient = null;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private ISettingsService settingsService;
        
        public CortexGraphClient(ISettingsService settingsService)
        {
            if (CortexGraphClient.httpClient == null)
                CortexGraphClient.httpClient = new HttpClient();

            this.settingsService = settingsService;
        }

        public async Task<IEnumerable<Neuron>> GetAll(string avatarUri)
        {
            var response = await CortexGraphClient.httpClient.GetAsync($"{avatarUri}{this.settingsService.GraphPath}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string data = (await response.Content.ReadAsStringAsync());
            JArray jd = JArray.Parse(data);
            var result = new List<Neuron>();
            foreach(var jt in jd.Children())
            {
                var ts = new List<Terminal>();
                foreach (var jc in JsonHelper.GetRequiredChildren((JObject)jt, "Terminals"))
                    ts.Add(new Terminal(
                        JsonHelper.GetRequiredValue<string>(jc, "TargetId"),
                        (NeurotransmitterEffect)Enum.Parse(typeof(NeurotransmitterEffect), JsonHelper.GetRequiredValue<string>(jc, "Effect")),
                        float.Parse(JsonHelper.GetRequiredValue<string>(jc, "Strength"))
                        ));
                result.Add(new Neuron(
                    JsonHelper.GetRequiredValue<string>(jt, "Id"),
                    JsonHelper.GetRequiredValue<string>(jt, "Data"),
                    ts.ToArray()
                    )
                );
            }
            return result.AsEnumerable();
        }
    }
}
