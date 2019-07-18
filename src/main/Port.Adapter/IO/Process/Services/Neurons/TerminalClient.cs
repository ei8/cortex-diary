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
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class TerminalClient : ITerminalClient
    {
        private readonly ISettingsService settingsService;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => TerminalClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static string terminalsPathTemplate = "cortex/terminals/{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TerminalClient(ISettingsService settingsService = null)
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public async Task CreateTerminal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken)) =>
            await TerminalClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateTerminalInternal(avatarUrl, id, presynapticNeuronId, postsynapticNeuronId, effect, strength, authorId, token).ConfigureAwait(false));

        public async Task CreateTerminalInternal(string avatarUrl, string id, string presynapticNeuronId, string postsynapticNeuronId, NeurotransmitterEffect effect, float strength, string authorId, CancellationToken token = default(CancellationToken))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"PresynapticNeuronId\": \"{presynapticNeuronId}\",");
            sb.Append($"\"PostsynapticNeuronId\": \"{postsynapticNeuronId}\",");
            sb.Append($"\"Effect\": \"{effect.ToString()}\",");
            sb.Append($"\"Strength\": \"{strength.ToString()}\"");
            Helper.AppendAuthorId(authorId, sb);
            sb.Append("}");

            await Helper.SendRequest(
                WebRequestMethods.Http.Put,
                avatarUrl,
                string.Format(TerminalClient.terminalsPathTemplate, id),
                sb,
                token
                );
        }
        
        public async Task DeactivateTerminal(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await TerminalClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.DeactivateTerminalInternal(avatarUrl, id, authorId, expectedVersion, token).ConfigureAwait(false));

        public async Task DeactivateTerminalInternal(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            Helper.AppendAuthorId(authorId, sb);
            sb.Append("}");

            await Helper.SendRequest(
                "DELETE",
                avatarUrl,
                string.Format(TerminalClient.terminalsPathTemplate, id),
                sb,
                token,
                new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
                );
        }
    }
}
