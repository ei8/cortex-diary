using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Diary.Nucleus.Client.Out;

namespace ei8.Cortex.Diary.Application.Notifications
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private readonly INotificationClient notificationClient;
 
        public NotificationApplicationService(INotificationClient notificationClient = null)
        {
            this.notificationClient = notificationClient ?? Locator.Current.GetService<INotificationClient>();
        }

        public async Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, string bearerToken, CancellationToken token = default(CancellationToken))
        {
            return await this.notificationClient.GetNotificationLog(avatarUrl, notificationLogId, bearerToken, token);            
        }
    }
}
