using ReactiveUI;
using ReactiveUI.Winforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms.Neurons
{
    public class NeuronWinForms : Neuron
    {
        protected override IReactiveList<Neuron> CreateChildNeurons()
        {
            return new ReactiveBindingList<Neuron>();
        }
    }
}
