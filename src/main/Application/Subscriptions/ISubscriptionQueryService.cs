using ei8.Cortex.Subscriptions.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Subscriptions
{
    public interface ISubscriptionQueryService
    {
        Task<SubscriptionConfiguration> GetServerConfigurationAsync(string avatarUrl, CancellationToken token = default);
    }
}
