using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Subscriptions.Common;
using ei8.Cortex.Subscriptions.Common.Receivers;
using System;
using System.Threading.Tasks;
namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public class SubscriptionApplicationService : ISubscriptionApplicationService
    {
		private readonly ISubscriptionClient subscriptionClient;
        private readonly ITokenManager tokenManager;

        public SubscriptionApplicationService(ISubscriptionClient subscriptionClient, ITokenManager tokenManager)
		{
			this.subscriptionClient = subscriptionClient;
            this.tokenManager = tokenManager;
        }

        public async Task SubscribeAsync(string avatarUrl, SubscriptionInfo subscription, IReceiverInfo receiverInfo)
        {
            var token = await this.tokenManager.RetrieveAccessTokenAsync();

            switch (receiverInfo)
            {
                case BrowserReceiverInfo br:
                    await this.subscriptionClient.AddSubscriptionAsync(
                        avatarUrl, 
                        new AddSubscriptionWebReceiverRequest()
                        {
                            SubscriptionInfo = subscription,
                            ReceiverInfo = br
                        }, 
                        token
                        );
                    break;
                case SmtpReceiverInfo sr:
                    await this.subscriptionClient.AddSubscriptionAsync(
                        avatarUrl, 
                        new AddSubscriptionSmtpReceiverRequest()
                        {
                            SubscriptionInfo = subscription,
                            ReceiverInfo = sr
                        }, 
                        token
                        );
                    break;
                default:
                    throw new NotSupportedException($"Unsupported receiver info type {receiverInfo.GetType().Name}");
            }
        }
    }
}
