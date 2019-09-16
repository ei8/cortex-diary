using org.neurul.Common.Events;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Application.Notifications
{
    public interface INotificationApplicationService
    {
        Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken));
    }
}
