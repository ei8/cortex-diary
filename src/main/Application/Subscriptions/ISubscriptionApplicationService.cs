using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public interface ISubscriptionApplicationService
    {
        Task SubscribeWithBrowserAsync(string avatarUrl, string avatarSnapshotUrl, string deviceName, string pushAuth, string pushP256dh, string pushEndpoint);
        Task SusbcribeWithEmailAsync(string avatarUrl, string avatarSnapshotUrl, string email);
        Task SusbcribeWithMobileAsync(string avatarUrl, string avatarSnapshotUrl, string mobileNumber);
    }
}
