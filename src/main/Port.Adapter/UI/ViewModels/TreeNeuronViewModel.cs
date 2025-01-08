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

        public TreeNeuronViewModel(Neuron neuron, string avatarUrl, INeuronQueryService neuronQueryService)
        {
            this.Neuron = neuron;
            this.avatarUrl = avatarUrl;
            this.neuronQueryService = neuronQueryService;
            this.Children = new List<TreeNeuronViewModel>();
            this.ExpandPostsynapticsUntilExternalReferencesTimer = new Timer();
        }

        public IList<TreeNeuronViewModel> Children { get; set; }

        public Neuron Neuron { get; private set; }

        public ExpansionState ExpansionState { get; private set; }

        public Timer ExpandPostsynapticsUntilExternalReferencesTimer { get; }

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
        //public async Task StartExpandPostsynapticsUntilExternalReferences(int expandTimeLimit)
        //{
        //    _cancellationTokenSource = new CancellationTokenSource(expandTimeLimit);

        //    try
        //    {
        //        await StartExpandPostsynapticsUntilExternalReferences(_cancellationTokenSource.Token);
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        // Handle the cancellation if needed
        //    }
        //}

        //private async Task StartExpandPostsynapticsUntilExternalReferences(CancellationToken cancellationToken)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();

        //    if (this.Neuron.Type != Library.Common.RelativeType.Presynaptic && string.IsNullOrEmpty(this.Neuron.ExternalReferenceUrl))
        //    {
        //        await Toggle();
        //        foreach (var item in this.Children)
        //        {
        //            cancellationToken.ThrowIfCancellationRequested();
        //            await item.StartExpandPostsynapticsUntilExternalReferences(cancellationToken);
        //        }
        //    }
        //}

        //public void CancelExpansion()
        //{
        //    _cancellationTokenSource?.Cancel();
        //}
    }
}
