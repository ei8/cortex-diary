using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Diary.Nucleus.Client.Out;
using ei8.Cortex.Diary.Domain.Model;

namespace ei8.Cortex.Diary.Application.Notifications
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private readonly INotificationClient notificationClient;
        private ITokenManager tokenManager;
 
        public NotificationApplicationService(INotificationClient notificationClient = null, ITokenManager tokenManager = null)
        {
            this.notificationClient = notificationClient ?? Locator.Current.GetService<INotificationClient>();
            this.tokenManager = tokenManager ?? Locator.Current.GetService<ITokenManager>();
        }

        public async Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken))
        {
            return await this.notificationClient.GetNotificationLog(avatarUrl, notificationLogId, await this.tokenManager.RetrieveAccessTokenAsync(), token);            
        }
    }
}
