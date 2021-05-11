using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Graph
{
    public class Link
    {
        public int source { get; set; }
        public int target { get; set; }
        public string type { get; set; }
    }
}
