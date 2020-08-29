using System.Linq;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Data
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
        }

        public NeuronViewModel[] Children { get; set; }

        public Neuron Neuron { get; private set; }

        public string Tag { get; private set; }

        public bool IsExpanded { get; private set; }

        public bool AreControlsVisible { get; private set; }
        
        public void Toggle()
        {
            this.IsExpanded = !this.IsExpanded;
        }

        public void ShowControls()
        {
            this.AreControlsVisible = true;
        }

        public void HideControls()
        {
            this.AreControlsVisible = false;
        }

        public string GetIcon()
        {
            if (IsExpanded)
            {
                return "-";
            }

            return "+";
        }

        public async Task OnReload()
        {
            this.IsExpanded = true;
            this.Children = (await this.neuronQueryService.GetNeurons(this.avatarUrl, this.Neuron.Id, new NeuronQuery())).Select(n => new NeuronViewModel(n, this.avatarUrl, this.neuronQueryService)).ToArray();
        }
    }
}
