using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Subscriptions.Common;
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

		public async Task SubscribeAsync(string avatarUrl, string avatarSnapshotUrl, string deviceName, string pushAuth, string pushP256dh, string pushEndpoint)
        {
			var token = await this.tokenManager.RetrieveAccessTokenAsync();

			await this.subscriptionClient.AddSubscriptionAsync(avatarUrl, new BrowserSubscriptionInfo()
			{
				AvatarUrl = avatarUrl,
				Name = deviceName,
				PushAuth = pushAuth,	
				PushEndpoint = pushEndpoint,
				PushP256DH = pushP256dh
			}, token);
        }
    }
}
