using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public class NeuronResultItemViewModel
    {
        public NeuronResultItemViewModel(Neuron neuron)
        {
            this.Neuron = neuron;
        }
        public Neuron Neuron { get; private set; }
    }    
}
