using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Settings;

namespace Blazor
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
