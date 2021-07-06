using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Port.Adapter.Common;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Common;
using IdentityModel.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels
{
    public class NeuronViewModel
    {
        private string avatarUrl;
        private INeuronQueryService neuronQueryService;

        public NeuronViewModel(Neuron neuron, string avatarUrl, INeuronQueryService neuronQueryService)
        {
            this.Neuron = neuron;
            this.Tag = neuron.Tag;
            this.avatarUrl = avatarUrl;
            this.neuronQueryService = neuronQueryService;
            this.Children = new List<NeuronViewModel>();
        }

        public IList<NeuronViewModel> Children { get; set; }

        public Neuron Neuron { get; private set; }

        public string Tag { get; private set; }

        public ExpansionState ExpansionState { get; private set; }

        public async Task Toggle()
        {
            this.ExpansionState = this.ExpansionState == ExpansionState.Collapsed ? ExpansionState.Expanding : ExpansionState.Collapsed;

            if (this.ExpansionState == ExpansionState.Expanding)
            {
                var children = new List<NeuronViewModel>();
                if (Library.Client.QueryUrl.TryParse(this.avatarUrl, out QueryUrl result))
                {
                    (await this.neuronQueryService.GetNeurons(result.AvatarUrl, this.Neuron.Id, new NeuronQuery()))
                        .Items
                        .ToList().ForEach(n =>
                        children.Add(new NeuronViewModel(new Neuron(n), this.avatarUrl, this.neuronQueryService))
                    );
                    this.Children = children.ToArray();
                }
                this.ExpansionState = ExpansionState.Expanded;
            }
        }
    }
}
