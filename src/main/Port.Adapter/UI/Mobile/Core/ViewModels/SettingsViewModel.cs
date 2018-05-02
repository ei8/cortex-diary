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
using System.Threading.Tasks;
using System.Windows.Input;
using works.ei8.Cortex.Diary.Application.Dialog;
using works.ei8.Cortex.Diary.Application.Locations;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.User;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Services.Dialog;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Services.Navigation;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels.Base;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _titleUseAzureServices;
        private string _descriptionUseAzureServices;
        private bool _useAzureServices;
        private string _titleUseFakeLocation;
        private string _descriptionUseFakeLocation;
        private bool _allowGpsLocation;
        private string _titleAllowGpsLocation;
        private string _descriptionAllowGpsLocation;
        private bool _useFakeLocation;
        private string _endpoint;
        private string avatarEndpoint;
        private double _latitude;
        private double _longitude;
        private string _gpsWarningMessage;

        private readonly ISettingsService _settingsService;
        private readonly ILocationService _locationService;

        public SettingsViewModel(ISettingsService settingsService, IDialogService dialogService, 
            INavigationService<ViewModelBase> navigationService, ILocationService locationService)
            : base(settingsService, dialogService, navigationService) 
        {
            _settingsService = settingsService;
            _locationService = locationService;

            _useAzureServices = !_settingsService.UseMocks;
            _endpoint = settingsService.BaseEndpoint;
            this.avatarEndpoint = settingsService.AvatarEndpoint;
            _latitude = double.Parse(_settingsService.Latitude, CultureInfo.CurrentCulture);
            _longitude = double.Parse(_settingsService.Longitude, CultureInfo.CurrentCulture);
            _useFakeLocation = _settingsService.UseFakeLocation;
            _allowGpsLocation = _settingsService.AllowGpsLocation;
            _gpsWarningMessage = string.Empty;
        }

        public string TitleUseAzureServices
        {
            get => _titleUseAzureServices;
            set
            {
                _titleUseAzureServices = value;
                this.OnPropertyChanged(nameof(TitleUseAzureServices));
            }
        }

        public string DescriptionUseAzureServices
        {
            get => _descriptionUseAzureServices;
            set
            {
                _descriptionUseAzureServices = value;
                this.OnPropertyChanged(nameof(DescriptionUseAzureServices));
            }
        }

        public bool UseAzureServices
        {
            get => _useAzureServices;
            set
            {
                _useAzureServices = value;

                UpdateUseAzureServices();

                this.OnPropertyChanged(nameof(UseAzureServices));
            }
        }

        public string TitleUseFakeLocation
        {
            get => _titleUseFakeLocation;
            set
            {
                _titleUseFakeLocation = value;
                this.OnPropertyChanged(nameof(TitleUseFakeLocation));
            }
        }

        public string DescriptionUseFakeLocation
        {
            get => _descriptionUseFakeLocation;
            set
            {
                _descriptionUseFakeLocation = value;
                this.OnPropertyChanged(nameof(DescriptionUseFakeLocation));
            }
        }

        public bool UseFakeLocation
        {
            get => _useFakeLocation;
            set
            {
                _useFakeLocation = value;

                UpdateFakeLocation();

                this.OnPropertyChanged(nameof(UseFakeLocation));
            }
        }

        public string TitleAllowGpsLocation
        {
            get => _titleAllowGpsLocation;
            set
            {
                _titleAllowGpsLocation = value;
                this.OnPropertyChanged(nameof(TitleAllowGpsLocation));
            }
        }

        public string DescriptionAllowGpsLocation
        {
            get => _descriptionAllowGpsLocation;
            set
            {
                _descriptionAllowGpsLocation = value;
                this.OnPropertyChanged(nameof(DescriptionAllowGpsLocation));
            }
        }

        public string GpsWarningMessage
        {
            get => _gpsWarningMessage;
            set
            {
                _gpsWarningMessage = value;
                this.OnPropertyChanged(nameof(GpsWarningMessage));
            }
        }

        public string Endpoint
        {
            get => _endpoint;
            set
            {
                _endpoint = value;

                if (!string.IsNullOrEmpty(_endpoint))
                {
                    UpdateEndpoint();
                }

                this.OnPropertyChanged(nameof(Endpoint));
            }
        }

        public string BrainEndpoint
        {
            get => avatarEndpoint;
            set
            {
                avatarEndpoint = value;

                if (!string.IsNullOrEmpty(avatarEndpoint))
                {
                    UpdateBrainEndpoint();
                }

                this.OnPropertyChanged(nameof(BrainEndpoint));
            }
        }

        public double Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;

                UpdateLatitude();

                this.OnPropertyChanged(nameof(Latitude));
            }
        }

        public double Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;

                UpdateLongitude();

                this.OnPropertyChanged(nameof(Longitude));
            }
        }

        public bool AllowGpsLocation
        {
            get => _allowGpsLocation;
            set
            {
                _allowGpsLocation = value;

                UpdateAllowGpsLocation();

                this.OnPropertyChanged(nameof(AllowGpsLocation));
            }
        }

        public bool UserIsLogged => !string.IsNullOrEmpty(_settingsService.AuthAccessToken);

        public ICommand ToggleMockServicesCommand => new Command(async () => await ToggleMockServicesAsync());

        public ICommand ToggleFakeLocationCommand => new Command(ToggleFakeLocationAsync);

        public ICommand ToggleSendLocationCommand => new Command(async () => await ToggleSendLocationAsync());

        public ICommand ToggleAllowGpsLocationCommand => new Command(ToggleAllowGpsLocation);

        public ICommand ClearCommand => new Command(Clear);

        private void Clear()
        {
            this.settingsService.Clear();
        }

        public override Task InitializeAsync(object navigationData)
        {
            UpdateInfoUseAzureServices();
            UpdateInfoFakeLocation();
            UpdateInfoAllowGpsLocation();

            return base.InitializeAsync(navigationData);
        }

        private async Task ToggleMockServicesAsync()
        {
            UpdateInfoUseAzureServices();

            var previousPageViewModel = NavigationService.PreviousPageViewModel;
            if (previousPageViewModel != null)
            {
                if (previousPageViewModel is MainViewModel)
                {
                    // Slight delay so that page navigation isn't instantaneous
                    await Task.Delay(1000);
                    if (UseAzureServices)
                    {
                        _settingsService.AuthAccessToken = string.Empty;
                        _settingsService.AuthIdToken = string.Empty;

                        await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
                        await NavigationService.RemoveBackStackAsync();
                    }
                }
            }
        }

        private void ToggleFakeLocationAsync()
        {
            UpdateInfoFakeLocation();
        }

        private async Task ToggleSendLocationAsync()
        {
            if (!_settingsService.UseMocks)
            {
                var locationRequest = new Location
                {
                    Latitude = _latitude,
                    Longitude = _longitude
                };
                var authToken = _settingsService.AuthAccessToken;

                await _locationService.UpdateUserLocation(locationRequest, authToken);
            }
        }

        private void ToggleAllowGpsLocation()
        {
            UpdateInfoAllowGpsLocation();
        }

        private void UpdateInfoUseAzureServices()
        {
            if (!UseAzureServices)
            {
                TitleUseAzureServices = "Use Mock Services";
                DescriptionUseAzureServices = "Mock Services are simulated objects that mimic the behavior of real services using a controlled approach.";
            }
            else
            {
                TitleUseAzureServices = "Use Microservices/Containers";
                DescriptionUseAzureServices = "When enabling the use of microservices/containers, the app will attempt to use real services deployed as Docker containers at the specified base endpoint, which will must be reachable through the network.";
            }
        }

        private void UpdateInfoFakeLocation()
        {
            if (!UseFakeLocation)
            {
                TitleUseFakeLocation = "Use Real Location";
                DescriptionUseFakeLocation = "When enabling location, the app will attempt to use the location from the device.";

            }
            else
            {
                TitleUseFakeLocation = "Use Fake Location";
                DescriptionUseFakeLocation = "Fake Location data is added for marketing campaign testing.";
            }
        }

        private void UpdateInfoAllowGpsLocation()
        {
            if (!AllowGpsLocation)
            {
                TitleAllowGpsLocation = "GPS Location Disabled";
                DescriptionAllowGpsLocation = "When disabling location, you won't receive location campaigns based upon your location.";
            }
            else
            {
                TitleAllowGpsLocation = "GPS Location Enabled";
                DescriptionAllowGpsLocation = "When enabling location, you'll receive location campaigns based upon your location.";
            }
        }

        private void UpdateUseAzureServices()
        {
            // Save use mocks services to local storage
            _settingsService.UseMocks = !_useAzureServices;
        }

        private void UpdateEndpoint()
        {
            // Update remote endpoint (save to local storage)
            this.settingsService.BaseEndpoint = _endpoint;
        }

        private void UpdateBrainEndpoint()
        {
            this.settingsService.AvatarEndpoint = this.avatarEndpoint;
        }

        private void UpdateFakeLocation()
        {
            _settingsService.UseFakeLocation = _useFakeLocation;
        }

        private void UpdateLatitude()
        {
            // Update fake latitude (save to local storage)
            _settingsService.Latitude = _latitude.ToString();
        }

        private void UpdateLongitude()
        {
            // Update fake longitude (save to local storage)
            _settingsService.Longitude = _longitude.ToString();
        }

        private void UpdateAllowGpsLocation()
        {
            if (_allowGpsLocation)
            {
                var locator = CrossGeolocator.Current;
                if (!locator.IsGeolocationEnabled)
                {
                    _allowGpsLocation = false;
                    GpsWarningMessage = "Enable the GPS sensor on your device";
                }
                else
                {
                    _settingsService.AllowGpsLocation = _allowGpsLocation;
                    GpsWarningMessage = string.Empty;
                }
            }
            else
            {
                _settingsService.AllowGpsLocation = _allowGpsLocation;
            }
        }
    }
}