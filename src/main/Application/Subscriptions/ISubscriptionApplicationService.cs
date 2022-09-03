using ei8.Cortex.Subscriptions.Common;
using ei8.Cortex.Subscriptions.Common.Receivers;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public interface ISubscriptionApplicationService
    {
        Task SubscribeAsync(string avatarUrl, SubscriptionInfo subscription, IReceiverInfo receiverInfo);
    }
}
