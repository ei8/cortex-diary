using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;

namespace works.ei8.Cortex.Diary.Domain.Model.Notifications
{
    // DEL: Use EventSourcing.Client.Out
    public interface INotificationClient
    {
        Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken));
    }
}
