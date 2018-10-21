using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.WinForms
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
            this.GraphPath = "/cortex/graph/neurons";
        }

        public string GraphPath { get; set; }
    }
}
