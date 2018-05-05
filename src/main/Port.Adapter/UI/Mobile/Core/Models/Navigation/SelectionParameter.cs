using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.Navigation
{
    public class SelectionParameter
    {
        public Action<Neuron> CompletionProcessor { get; set; }
        public Func<Neuron, Task> CompletionProcessorAsync { get; set; }
    }
}
