using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public interface IPluginSettingsService
    {
        public IConfiguration Configuration { get; set; }
    }
}
