using Splat;
using System;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Client;
using works.ei8.EventSourcing.Common;

namespace works.ei8.Cortex.Diary.Application.Notifications
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private IEventSourceFactory eventSourceFactory;

        public NotificationApplicationService(IEventSourceFactory eventSourceFactory = null)
        {
            this.eventSourceFactory = this.eventSourceFactory ?? Locator.Current.GetService<IEventSourceFactory>();
        }

        public async Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken))
        {
            return await this.eventSourceFactory.Create(string.Empty, avatarUrl, Guid.Empty).NotificationClient.GetNotificationLog(notificationLogId, token);
        }
    }
}
