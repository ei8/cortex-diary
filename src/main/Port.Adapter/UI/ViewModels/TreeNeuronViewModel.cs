using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public enum ExpansionType
    {
        None,
        PostsynapticUntilExternalReferences,
        FarthestPresynaptic
    }

    public class TreeNeuronViewModel
    {
        private string avatarUrl;
        private INeuronQueryService neuronQueryService;
        private Timer expansionTimer;
        private ExpansionType currentExpansionType = ExpansionType.None;

        public TreeNeuronViewModel(Neuron neuron, string avatarUrl, INeuronQueryService neuronQueryService)
        {
            this.Neuron = neuron;
            this.avatarUrl = avatarUrl;
            this.neuronQueryService = neuronQueryService;
            this.Children = new List<TreeNeuronViewModel>();
            this.expansionTimer = new Timer();
        }

        public IList<TreeNeuronViewModel> Children { get; set; }

        public Neuron Neuron { get; private set; }

        public ExpansionState ExpansionState { get; private set; }

        public ExpansionType CurrentExpansionType => this.currentExpansionType;

        public async Task Toggle()
        {
            this.ExpansionState = this.ExpansionState == ExpansionState.Collapsed ? ExpansionState.Expanding : ExpansionState.Collapsed;

            if (this.ExpansionState == ExpansionState.Expanding)
            {
                var children = new List<TreeNeuronViewModel>();
                if (Library.Client.QueryUrl.TryParse(this.avatarUrl, out QueryUrl result))
                {
                    NeuronQuery.TryParse(result.QueryString, out NeuronQuery query);
                    var childrenQuery = new NeuronQuery()
                    {
                        PageSize = Constants.TreeNodeChildrenQueryPageSize,
                        RelativeValues = query != null ? query.RelativeValues : null
                    };
                    (await this.neuronQueryService.GetNeurons(result.AvatarUrl, this.Neuron.Id, childrenQuery))
                        .Items
                        .ToList().ForEach(n =>
                        children.Add(new TreeNeuronViewModel(new Neuron(n), this.avatarUrl, this.neuronQueryService))
                    );
                    this.Children = children.ToArray();
                }
                this.ExpansionState = ExpansionState.Expanded;
            }
        }

        public void ConfigureExpansionTimer(ExpansionType type, double interval, ElapsedEventHandler handler)
        {
            this.currentExpansionType = type;
            this.expansionTimer.Interval = interval;
            this.expansionTimer.Elapsed -= handler;
            this.expansionTimer.Elapsed += handler;
        }

        public void StartExpansionTimer()
        {
            this.expansionTimer.Start();
        }

        public void StopExpansionTimer()
        {
            this.expansionTimer.Stop();
            this.currentExpansionType = ExpansionType.None;
        }

        public void RestartExpansionTimer()
        {
            this.expansionTimer.Enabled = false;
            this.StartExpansionTimer();
        }

        public bool IsExpansionTimerEnabled()
        {
            return this.expansionTimer.Enabled;
        }

        public bool IsChild(string id, RelativeType type)
        {
            var result = this.Children.Any(x => (x.Neuron.Id == id && x.Neuron.Type ==type) || x.IsChild(id, type));
            return result;
        }
    }
}
