using org.neurul.Common.Events;
using Splat;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Notifications;

namespace works.ei8.Cortex.Diary.Application.Notifications
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        private INotificationClient notificationClient;

        public NotificationApplicationService(INotificationClient notificationClient = null)
        {
            this.notificationClient = notificationClient ?? Locator.Current.GetService<INotificationClient>();
        }

        public async Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken))
        {
            return await this.notificationClient.GetNotificationLog(avatarUrl, notificationLogId, token);
        }
    }
}
