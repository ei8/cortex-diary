using Microsoft.Extensions.Configuration;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public interface IPluginSettingsService
    {
        public IConfiguration Configuration { get; set; }
    }
}
