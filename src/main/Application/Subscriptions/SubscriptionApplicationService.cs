using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Subscriptions.Common;
using ei8.Cortex.Subscriptions.Common.Receivers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public class SubscriptionApplicationService : ISubscriptionApplicationService
    {
		private readonly IServiceProvider services;
        private readonly ITokenManager tokenManager;

        public SubscriptionApplicationService(IServiceProvider services, ITokenManager tokenManager)
		{
			this.services = services;
            this.tokenManager = tokenManager;
        }

		public async Task SubscribeWithBrowserAsync(string avatarUrl, string avatarSnapshotUrl, string deviceName, string pushAuth, string pushP256dh, string pushEndpoint)
        {
			var token = await this.tokenManager.RetrieveAccessTokenAsync();
            var subscriptionClient = this.services.GetRequiredService<ISubscriptionClient<BrowserReceiverInfo>>();

			await subscriptionClient.AddSubscriptionAsync(avatarUrl, new AddSubscriptionWebReceiverRequest()
			{
                SubscriptionInfo = new SubscriptionInfo()
                {
                    AvatarUrl = avatarSnapshotUrl
                },
                ReceiverInfo = new BrowserReceiverInfo() 
                {
                    PushAuth = pushAuth,
                    PushEndpoint = pushEndpoint,
                    PushP256DH = pushP256dh,
                    Name = deviceName,
                }
			}, token);
        }

        // TODO
        public Task SusbcribeWithEmailAsync(string avatarUrl, string avatarSnapshotUrl, string email)
        {
            throw new System.NotImplementedException();
        }

        // TODO
        public Task SusbcribeWithMobileAsync(string avatarUrl, string avatarSnapshotUrl, string mobileNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
