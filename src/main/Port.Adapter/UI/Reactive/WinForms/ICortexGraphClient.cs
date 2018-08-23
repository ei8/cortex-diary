using Spiker.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms
{
    public interface ICortexGraphClient
    {
        Task<IEnumerable<Neuron>> GetAll(string avatarUri);
    }
}
