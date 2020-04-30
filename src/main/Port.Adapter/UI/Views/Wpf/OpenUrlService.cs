using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.OpenUrl;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class OpenUrlService : IOpenUrlService
    {
        public void OpenUrl(string url)
        {
            Process.Start(url);
        }
    }
}
