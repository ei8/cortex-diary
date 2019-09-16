using org.neurul.Common.Events;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Domain.Model.Notifications
{
    public interface INotificationClient
    {
        Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken));
    }
}
