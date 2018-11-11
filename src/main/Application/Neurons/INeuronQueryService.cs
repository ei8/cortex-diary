using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public interface INeuronQueryService
    {
        Task<IEnumerable<Neuron>> GetNeurons(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = default(string), int? limit = 1000, CancellationToken token = default(CancellationToken));

        Task<IEnumerable<Neuron>> GetNeuronById(string avatarUrl, string id, Neuron central = null, RelativeType type = RelativeType.NotSet, CancellationToken token = default(CancellationToken));
    }
}
