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

using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels.Base;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Views;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Services.Navigation
{
    public class NavigationService : INavigationService<ViewModelBase>
    {
        private readonly ISettingsService _settingsService;

        public ViewModelBase PreviousPageViewModel
        {
            get
            {
                var mainPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;
                var viewModel = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2].BindingContext;
                return viewModel as ViewModelBase;
            }
        }

        public NavigationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(_settingsService.AuthAccessToken))
                return NavigateToAsync<LoginViewModel>();
            else
                return NavigateToAsync<MainViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task RemoveLastFromBackStackAsync()
        {
            var mainPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                mainPage.Navigation.RemovePage(
                    mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        public Task RemoveBackStackAsync()
        {
            var mainPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;

            if (mainPage != null)
            {
                for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                {
                    var page = mainPage.Navigation.NavigationStack[i];
                    mainPage.Navigation.RemovePage(page);
                }
            }

            return Task.FromResult(true);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreatePage(viewModelType, parameter);

            if (page is LoginView)
            {
                Xamarin.Forms.Application.Current.MainPage = new CustomNavigationView(page);
            }
            else
            {
                var navigationPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;
                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    Xamarin.Forms.Application.Current.MainPage = new CustomNavigationView(page);
                }
            }

            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }

        public Task ModalNavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return this.InternalModalNavigateToAsync(typeof(TViewModel), parameter);
        }

        private async Task InternalModalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreatePage(viewModelType, parameter);

            var navigationPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;
            if (navigationPage != null)
            {
                await navigationPage.Navigation.PushModalAsync(page);
            }
            else
            {
                Xamarin.Forms.Application.Current.MainPage = new CustomNavigationView(page);
            }

            await(page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        public async Task ModalNavigateFrom()
        {
            var navigationPage = Xamarin.Forms.Application.Current.MainPage as CustomNavigationView;
            await navigationPage.Navigation.PopModalAsync();
        }

        public async Task NavigateBack()
        {
            await Xamarin.Forms.Application.Current.MainPage.Navigation.PopAsync();
        }

        public bool CanNavigateBack => Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.Count > 1;
    }
}