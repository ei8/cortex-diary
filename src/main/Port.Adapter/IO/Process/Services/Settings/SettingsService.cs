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

using Splat;
using System;
using System.Collections.Generic;
using ei8.Cortex.Diary.Application.Dependency;
using ei8.Cortex.Diary.Application.Settings;
using IdentityServer4.Models;
using ei8.Cortex.Diary.Port.Adapter.Common;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsServiceImplementation _settingsServiceImpl;

        ISettingsServiceImplementation AppSettings
        {
            get { return _settingsServiceImpl; }
        }

        public SettingsService(IDependencyService dependencyService = null)
        {
            _settingsServiceImpl = (dependencyService ?? Locator.Current.GetService<IDependencyService>()).Get<ISettingsServiceImplementation>();
        }

        #region Setting Constants

        private const string IdUseMocks = "use_mocks";
        private const string IdUseFakeLocation = "use_fake_location";
        private const string IdLatitude = "latitude";
        private const string IdLongitude = "longitude";
        private const string IdAllowGpsLocation = "allow_gps_location";
        private const string IdOidcAuthority = "oidc_authority";
        private const string IdClientId = "client_id";
        private const string IdClientSecret = "client_secret";
        private const string IdDatabasePath = "database_path";
        private const string IdBasePath = "base_path";
        private const string IdPluginsPath = "plugins_path";
        private const string IdValidateServerCertificate = "validate_server_certificate";
        private const string IdLoginCallback = "login_callback";
        private const string IdLogoutCallback = "logout_callback";
        private const string IdApplicationUrl = "application_url";
        private const string IdAppTitle = "app_title";
        private const string IdAppIcon = "app_icon";

        private readonly bool UseMocksDefault = true;
        private readonly bool UseFakeLocationDefault = false;
        private readonly double FakeLatitudeDefault = 47.604610d;
        private readonly double FakeLongitudeDefault = -122.315752d;
        private readonly bool AllowGpsLocationDefault = false;
        private readonly string OidcAuthorityDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.OidcAuthority);
        private readonly string ClientIdDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ClientId);
        private readonly string ClientSecretDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ClientSecret);
        private readonly string DatabasePathDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath);
        private readonly string BasePathDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.BasePath);
        private readonly string PluginsPathDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.PluginsPath);
        private readonly string AppTitleDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.AppTitle);
        private readonly string AppIconDefault = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.AppIcon);
        private readonly bool ValidateServerCertificateDefault = 
            bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ValidateServerCertificate), out bool result) ? 
                result : 
                true;
        private readonly string LoginCallbackDefault = string.Empty;
        private readonly string LogoutCallbackDefault = string.Empty;
        private readonly string ApplicationUrlDefault = string.Empty;
        #endregion

        #region Application
        public string ApplicationUrl
        {
            get => AppSettings.GetValueOrDefault(IdApplicationUrl, ApplicationUrlDefault);
            set
            {
                AppSettings.AddOrUpdateValue(IdApplicationUrl, value);
                this.UpdateAppEndpoint(this.ApplicationUrl);
            }
        }

        public string LoginCallback
        {
            get => AppSettings.GetValueOrDefault(IdLoginCallback, LoginCallbackDefault);
            set => AppSettings.AddOrUpdateValue(IdLoginCallback, value);
        }

        public string LogoutCallback
        {
            get => AppSettings.GetValueOrDefault(IdLogoutCallback, LogoutCallbackDefault);
            set => AppSettings.AddOrUpdateValue(IdLogoutCallback, value);
        }

        public string OidcAuthority
        {
            get => AppSettings.GetValueOrDefault(IdOidcAuthority, OidcAuthorityDefault);
            set => AppSettings.AddOrUpdateValue(IdOidcAuthority, value);
        }

        public string ClientId
        {
            get => AppSettings.GetValueOrDefault(IdClientId, ClientIdDefault);
            set => AppSettings.AddOrUpdateValue(IdClientId, value);
        }

        public string ClientSecret
        {
            get => AppSettings.GetValueOrDefault(IdClientSecret, ClientSecretDefault);
            set => AppSettings.AddOrUpdateValue(IdClientSecret, value);
        }

        public string DatabasePath
        {
            get => AppSettings.GetValueOrDefault(IdDatabasePath, DatabasePathDefault);
            set => AppSettings.AddOrUpdateValue(IdDatabasePath, value);
        }

        public string BasePath
        {
            get => AppSettings.GetValueOrDefault(IdBasePath, BasePathDefault);
            set => AppSettings.AddOrUpdateValue(IdBasePath, value);
        }

        public string PluginsPath
        {
            get => AppSettings.GetValueOrDefault(IdPluginsPath, PluginsPathDefault);
            set => AppSettings.AddOrUpdateValue(IdPluginsPath, value);
        }

        public string AppTitle
        {
            get => AppSettings.GetValueOrDefault(IdAppTitle, AppTitleDefault);
            set => AppSettings.AddOrUpdateValue(IdAppTitle, value);
        }
        public string AppIcon
        {
            get => AppSettings.GetValueOrDefault(IdAppIcon, AppIconDefault);
            set => AppSettings.AddOrUpdateValue(IdAppIcon, value);
        }

        public bool ValidateServerCertificate
        {
            get => AppSettings.GetValueOrDefault(IdValidateServerCertificate, ValidateServerCertificateDefault);
            set => AppSettings.AddOrUpdateValue(IdValidateServerCertificate, value);
        }
        #endregion

        #region [Unused]
        public bool UseMocks
        {
            get => AppSettings.GetValueOrDefault(IdUseMocks, UseMocksDefault);
            set => AppSettings.AddOrUpdateValue(IdUseMocks, value);
        }

        public bool UseFakeLocation
        {
            get => AppSettings.GetValueOrDefault(IdUseFakeLocation, UseFakeLocationDefault);
            set => AppSettings.AddOrUpdateValue(IdUseFakeLocation, value);
        }

        public string Latitude
        {
            get => AppSettings.GetValueOrDefault(IdLatitude, FakeLatitudeDefault.ToString());
            set => AppSettings.AddOrUpdateValue(IdLatitude, value);
        }

        public string Longitude
        {
            get => AppSettings.GetValueOrDefault(IdLongitude, FakeLongitudeDefault.ToString());
            set => AppSettings.AddOrUpdateValue(IdLongitude, value);
        }

        public bool AllowGpsLocation
        {
            get => AppSettings.GetValueOrDefault(IdAllowGpsLocation, AllowGpsLocationDefault);
            set => AppSettings.AddOrUpdateValue(IdAllowGpsLocation, value);
        }
        #endregion

        private void UpdateAppEndpoint(string appUrl)
        {
            LoginCallback = $"{appUrl}/Account/LoginCallback";
            LogoutCallback = $"{appUrl}/Account/LogoutCallback";
        }

        public void Clear()
        {
            this.AppSettings.Remove(IdUseMocks);
            this.AppSettings.Remove(IdUseFakeLocation);
            this.AppSettings.Remove(IdLatitude);
            this.AppSettings.Remove(IdLongitude);
            this.AppSettings.Remove(IdAllowGpsLocation);
            this.AppSettings.Remove(IdOidcAuthority);
            this.AppSettings.Remove(IdClientId);
            this.AppSettings.Remove(IdClientSecret);
            this.AppSettings.Remove(IdLoginCallback);
            this.AppSettings.Remove(IdApplicationUrl);
            this.AppSettings.Remove(IdLogoutCallback);
        }

        void ISettingsService.Clear()
        {
            throw new NotImplementedException();
        }
    }
}