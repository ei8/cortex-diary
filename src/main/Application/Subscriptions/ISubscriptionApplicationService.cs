using System;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public interface ISubscriptionApplicationService
    {
        Task SubscribeAsync(string avatarUrl, string avatarSnapshotUrl, string deviceName, string pushAuth, string pushP256dh, string pushEndpoint);
    }
}
