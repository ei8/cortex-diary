//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using Plugin.Geolocator;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using TinyIoC;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Dialog;
using works.ei8.Cortex.Diary.Application.Identity;
using works.ei8.Cortex.Diary.Application.Locations;
using works.ei8.Cortex.Diary.Application.Message;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.OpenUrl;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Locations;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Message;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.OpenUrl;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Services.Dialog;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Services.Navigation;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels.Base
{
    public static class ViewModelLocator
    {
        private static TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ViewModelLocator.AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(ViewModelLocator.AutoWireViewModelProperty, value);
        }

        internal static async Task AppStartResume(bool initNavigation, bool processLocation)
        {
            if (initNavigation)
            {
                var navigationService = ViewModelLocator._container.Resolve<INavigationService<ViewModelBase>>();
                await navigationService.InitializeAsync();
            }

            if (processLocation)
            {
                var settingsService = ViewModelLocator._container.Resolve<ISettingsService>();
                if (settingsService.AllowGpsLocation && !settingsService.UseFakeLocation)
                {
                    await GetGpsLocation();
                }

                if (!settingsService.UseMocks && !string.IsNullOrEmpty(settingsService.AuthAccessToken))
                {
                    await SendCurrentLocation();
                }
            }
        }

        private static async Task GetGpsLocation()
        {
            var locator = CrossGeolocator.Current;
            var settingsService = ViewModelLocator._container.Resolve<ISettingsService>();

            if (locator.IsGeolocationEnabled && locator.IsGeolocationAvailable)
            {
                locator.AllowsBackgroundUpdates = true;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync();

                settingsService.Latitude = position.Latitude.ToString();
                settingsService.Longitude = position.Longitude.ToString();
            }
            else
            {
                settingsService.AllowGpsLocation = false;
            }
        }

        private static async Task SendCurrentLocation()
        {
            var settingsService = ViewModelLocator._container.Resolve<ISettingsService>();
            var location = new Location
            {
                Latitude = double.Parse(settingsService.Latitude, CultureInfo.InvariantCulture),
                Longitude = double.Parse(settingsService.Longitude, CultureInfo.InvariantCulture)
            };

            var locationService = ViewModelLocator._container.Resolve<ILocationService>();
            await locationService.UpdateUserLocation(location, settingsService.AuthAccessToken);
        }

        public static bool UseMockService { get; set; }

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();

            _container.Register<IDependencyService, Services.Dependency.DependencyService>();
            _container.Register<ISettingsService, SettingsService>().AsSingleton();
            _container.Register<INavigationService<ViewModelBase>, NavigationService>().AsSingleton();
            _container.Register<IDialogService, DialogService>();
            _container.Register<IOpenUrlService, OpenUrlService>();
            _container.Register<IIdentityService, IdentityService>();
            _container.Register<IRequestProvider, RequestProvider>();
            _container.Register<ILocationService, LocationService>().AsSingleton();
            _container.Register<IMessageService, MessageService>().AsSingleton();

            _container.Register<INeuronClient, NeuronClient>().AsSingleton();
            _container.Register<INeuronApplicationService, NeuronApplicationService>().AsSingleton();
            _container.Register<INeuronGraphQueryClient, NeuronGraphQueryClient>().AsSingleton();
            _container.Register<INeuronQueryService, NeuronQueryService>().AsSingleton();

            // View models
            _container.Register<LoginViewModel>();
            _container.Register<MainViewModel>();
            _container.Register<SettingsViewModel>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null)
            {
                return;
            }

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null)
            {
                return;
            }

            object viewModel = null;

            viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}