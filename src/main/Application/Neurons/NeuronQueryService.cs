using DynamicData;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public class NeuronQueryService : INeuronQueryService
    {
        private INeuronGraphQueryClient neuronQueryClient;

        public NeuronQueryService(INeuronGraphQueryClient neuronQueryClient = null)
        {
            this.neuronQueryClient = neuronQueryClient ?? Locator.Current.GetService<INeuronGraphQueryClient>();
        }

        public async Task<IEnumerable<Neuron>> GetNeuronById(string avatarUrl, string id, Neuron central = null, RelativeType type = RelativeType.NotSet, CancellationToken token = default(CancellationToken))
        {
            var neurons = await this.neuronQueryClient.GetNeuronById(avatarUrl, id, central, type, token);
            neurons.ToList().ForEach(n => {
                n.Id = Guid.NewGuid().GetHashCode();
                n.CentralId = central != null ? central.Id : int.MinValue;
            });

            return neurons;
        }

        public async Task<IEnumerable<Neuron>> GetNeurons(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, string filter = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            var relatives = await this.neuronQueryClient.GetNeurons(avatarUrl, central, type, filter, limit, token);
            relatives.ToList().ForEach(n => {
                n.Id = Guid.NewGuid().GetHashCode();
                n.CentralId = central != null ? central.Id : int.MinValue;
            });

            if (central == null)
                relatives = relatives.OrderBy(n => n.Data);
            else
            {
                var posts = relatives.Where(n => n.Type == RelativeType.Postsynaptic);
                var pres = relatives.Where(n => n.Type == RelativeType.Presynaptic);
                posts = posts.ToList().OrderBy(n => n.Data);
                pres = pres.ToList().OrderBy(n => n.Data);
                relatives = posts.Concat(pres);
            }

            return relatives;
        }
    }
}
