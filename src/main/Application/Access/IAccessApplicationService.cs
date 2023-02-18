using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ei8.Cortex.Diary.Application.Access
{
    public interface IAccessApplicationService
    {
        Task RequestNeuronAccessAsync(string avatarUrl, string neuronId, CancellationToken token = default);
    }
}
