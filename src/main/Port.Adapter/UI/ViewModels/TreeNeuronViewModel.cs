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
    public class TreeNeuronViewModel
    {
        private string avatarUrl;
        private INeuronQueryService neuronQueryService;
        private Timer expandPostsynapticsUntilExternalReferencesTimer;

        public TreeNeuronViewModel(Neuron neuron, string avatarUrl, INeuronQueryService neuronQueryService)
        {
            this.Neuron = neuron;
            this.avatarUrl = avatarUrl;
            this.neuronQueryService = neuronQueryService;
            this.Children = new List<TreeNeuronViewModel>();
            this.expandPostsynapticsUntilExternalReferencesTimer = new Timer();
        }

        public IList<TreeNeuronViewModel> Children { get; set; }

        public Neuron Neuron { get; private set; }

        public ExpansionState ExpansionState { get; private set; }

        public async Task Toggle()
        {
            this.ExpansionState = this.ExpansionState == ExpansionState.Collapsed ? ExpansionState.Expanding : ExpansionState.Collapsed;

            if (this.ExpansionState == ExpansionState.Expanding)
            {
                var children = new List<TreeNeuronViewModel>();
                if (Library.Client.QueryUrl.TryParse(this.avatarUrl, out QueryUrl result))
                {
                    (await this.neuronQueryService.GetNeurons(result.AvatarUrl, this.Neuron.Id, new NeuronQuery() { PageSize = Constants.TreeNodeChildrenQueryPageSize }))
                        .Items
                        .ToList().ForEach(n =>
                        children.Add(new TreeNeuronViewModel(new Neuron(n), this.avatarUrl, this.neuronQueryService))
                    );
                    this.Children = children.ToArray();
                }
                this.ExpansionState = ExpansionState.Expanded;
            }
        }

        public void ConfigureExpandTimer(double interval, ElapsedEventHandler handler)
        {
            this.expandPostsynapticsUntilExternalReferencesTimer.Interval = interval;
            this.expandPostsynapticsUntilExternalReferencesTimer.Elapsed -= handler;
            this.expandPostsynapticsUntilExternalReferencesTimer.Elapsed += handler;
        }

        public void StartExpandTimer()
        {
            this.expandPostsynapticsUntilExternalReferencesTimer.Start();
        }

        public void StopExpandTimer()
        {
            this.expandPostsynapticsUntilExternalReferencesTimer.Stop();
        }

        public void RestartExpandTimer()
        {
            this.expandPostsynapticsUntilExternalReferencesTimer.Enabled = false;
            this.StartExpandTimer();
        }

        public bool IsExpandTimerEnabled()
        {
            return this.expandPostsynapticsUntilExternalReferencesTimer.Enabled;
        }
    }
}
