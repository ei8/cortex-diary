﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public interface INeuronClient
    {
        Task AddTerminalsToNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task CreateNeuron(string avatarUrl, string id, string tag, IEnumerable<Terminal> terminals, string authorId, CancellationToken token = default(CancellationToken));

        Task ChangeNeuronTag(string avatarUrl, string id, string tag, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task RemoveTerminalsFromNeuron(string avatarUrl, string id, IEnumerable<Terminal> terminals, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken));

        Task DeactivateNeuron(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken));
    }
}
