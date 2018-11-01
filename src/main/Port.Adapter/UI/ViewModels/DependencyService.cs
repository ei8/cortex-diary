using Splat;
using works.ei8.Cortex.Diary.Application.Dependency;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public class DependencyService : IDependencyService
    {
        public T Get<T>() where T : class
        {
            return Locator.Current.GetService<T>();
        }
    }
}
