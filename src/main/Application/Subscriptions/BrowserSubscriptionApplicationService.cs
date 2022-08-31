using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Subscriptions.Common;
using ei8.Cortex.Subscriptions.Common.Receivers;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public class BrowserSubscriptionApplicationService : ISubscriptionApplicationService<BrowserReceiverInfo>
    {
		private readonly ISubscriptionClient<BrowserReceiverInfo> client;
        private readonly ITokenManager tokenManager;

        public BrowserSubscriptionApplicationService(ISubscriptionClient<BrowserReceiverInfo> browserReceiverClient, ITokenManager tokenManager)
		{
			this.client = browserReceiverClient;
            this.tokenManager = tokenManager;
        }

        public async Task SubscribeAsync(string avatarUrl, SubscriptionInfo subscription, BrowserReceiverInfo receiverInfo)
        {
            var token = await this.tokenManager.RetrieveAccessTokenAsync();

            await this.client.AddSubscriptionAsync(avatarUrl, new AddSubscriptionWebReceiverRequest()
            {
                SubscriptionInfo = subscription,
                ReceiverInfo = receiverInfo
            }, token);
        }
    }
}
