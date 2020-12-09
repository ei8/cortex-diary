using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Data
{
    public class EditorNeuronViewModel
    {
        public EditorNeuronViewModel() => this.Initialize();

        public string Id { get; set; }
        public string Tag { get; set; }
        public int Version { get; set; }

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Tag = string.Empty;
            this.Version = 0;
        }
    }
}
