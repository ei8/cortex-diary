using ei8.Cortex.Diary.Application.Dependency;
using ei8.Cortex.Diary.Application.Settings;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public class DependencyService : IDependencyService
    {
        private ISettingsServiceImplementation settingsServiceImplementation;

        public DependencyService(ISettingsServiceImplementation settingsServiceImplementation)
        {
            this.settingsServiceImplementation = settingsServiceImplementation;
        }

        public T Get<T>() where T : class
        {
            T result = default;
            if (typeof(T) == typeof(ISettingsServiceImplementation))
                result = (T) this.settingsServiceImplementation;

            return result;  
        }
    }
}
