using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons
{
    public enum NeurotransmitterEffect
    {
        Inhibit = -1,
        NotSet = 0,
        Excite = 1
    }

    public enum ChildType
    {
        Postsynaptic,
        Presynaptic
    }
}
