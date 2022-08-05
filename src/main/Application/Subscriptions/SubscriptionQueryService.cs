using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Nucleus.Client.Out;
using ei8.Cortex.Subscriptions.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public class SubscriptionQueryService : ISubscriptionQueryService
    {
        private readonly ISubscriptionConfigurationClient client;
        private readonly ISettingsService settings;

        public SubscriptionQueryService(ISubscriptionConfigurationClient client, ISettingsService settings)
        {
            this.client = client;
            this.settings = settings;
        }

        public async Task<SubscriptionConfiguration> GetServerConfigurationAsync(string avatarUrl, CancellationToken token = default)
        {
            return await this.client.GetServerConfigurationAsync(avatarUrl);
        }
    }
}
