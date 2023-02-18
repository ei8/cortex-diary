using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Subscriptions.Common.Receivers;
using ei8.Cortex.Subscriptions.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application.Access
{
    public class AccessApplicationService : IAccessApplicationService
    {
        private readonly IAccessClient accessClient;
        private readonly ITokenManager tokenManager;

        public AccessApplicationService(IAccessClient accessClient, ITokenManager tokenManager)
        {
            this.accessClient = accessClient;
            this.tokenManager = tokenManager;
        }

        public async Task RequestNeuronAccessAsync(string avatarUrl, string neuronId, CancellationToken token = default)
        {
            var accessToken = await this.tokenManager.RetrieveAccessTokenAsync();

            await this.accessClient.CreateNeuronAccessRequest(
                        avatarUrl,
                        neuronId,
                        accessToken,
                        token
                        );
        }
    }
}
