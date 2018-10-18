using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons
{
    public class PostsynapticViewModel : NeuronViewModelBase
    {
        public PostsynapticViewModel(string data, Node<NeuronDto, int> node, SourceCache<NeuronDto, int> cache, NeuronViewModelBase parent = null, IExtendedSelectionService selectionService = null) : base(node, cache, parent, selectionService)
        {
            this.Data = data;
        }

        public override object ViewModel => this;
    }
}
