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
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Settings;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings
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

        private const string IdAccessToken = "access_token";
        private const string IdToken = "id_token";
        private const string IdUseMocks = "use_mocks";
        private const string IdUseFakeLocation = "use_fake_location";
        private const string IdLatitude = "latitude";
        private const string IdLongitude = "longitude";
        private const string IdAllowGpsLocation = "allow_gps_location";
        private const string IdBaseEndpoint = "base_endpoint";
        private const string IdClientId = "client_id";
        private const string IdClientSecret = "client_secret";
        private const string IdAuthToken = "auth_token";
        private const string IdRegisterWebsite = "register_website";
        private const string IdIdentityEndpoint = "identity_endpoint";
        private const string IdLocationEndpoint = "location_endpoint";
        private const string IdTokenEndpoint = "token_endpoint";
        private const string IdLogoutEndpoint = "logout_endpoint";
        private const string IdIdentityCallback = "identity_callback";
        private const string IdLogoutCallback = "logout_callback";
        private const string IdRevocationEndpoint = "revocation_endpoint";

        private readonly string AccessTokenDefault = string.Empty;
        private readonly string IdTokenDefault = string.Empty;
        private readonly bool UseMocksDefault = true;
        private readonly bool UseFakeLocationDefault = false;
        private readonly double FakeLatitudeDefault = 47.604610d;
        private readonly double FakeLongitudeDefault = -122.315752d;
        private readonly bool AllowGpsLocationDefault = false;
        private readonly string ClientIdDefault = "xamarin";
        private readonly string ClientSecretDefault = "secret";
        private readonly string AuthTokenDefault = "INSERT AUTHENTICATION TOKEN";
        private readonly string RegisterWebsiteDefault = string.Empty;
        private readonly string IdentityEndpointDefault = string.Empty;
        private readonly string LocationEndpointDefault = string.Empty;
        private readonly string TokenEndpointDefault = string.Empty;
        private readonly string LogoutEndpointDefault = string.Empty;
        private readonly string IdentityCallbackDefault = string.Empty;
        private readonly string LogoutCallbackDefault = string.Empty;
        private readonly string RevocationEndpointDefault = string.Empty;
        
        #endregion

        public string AuthAccessToken
        {
            get => AppSettings.GetValueOrDefault(IdAccessToken, AccessTokenDefault);
            set => AppSettings.AddOrUpdateValue(IdAccessToken, value);
        }

        public string AuthIdToken
        {
            get => AppSettings.GetValueOrDefault(IdToken, IdTokenDefault);
            set => AppSettings.AddOrUpdateValue(IdToken, value);
        }

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

        public string AuthToken
        {
            get => AppSettings.GetValueOrDefault(IdAuthToken, AuthTokenDefault);
            set => AppSettings.AddOrUpdateValue(IdAuthToken, value);
        }

        public string RegisterWebsite
        {
            get => AppSettings.GetValueOrDefault(IdRegisterWebsite, RegisterWebsiteDefault);
            set => AppSettings.AddOrUpdateValue(IdRegisterWebsite, value);
        }

        public string IdentityEndpoint
        {
            get => AppSettings.GetValueOrDefault(IdIdentityEndpoint, IdentityEndpointDefault);
            set => AppSettings.AddOrUpdateValue(IdIdentityEndpoint, value);
        }

        public string LocationEndpoint
        {
            get => AppSettings.GetValueOrDefault(IdLocationEndpoint, LocationEndpointDefault);
            set => AppSettings.AddOrUpdateValue(IdLocationEndpoint, value);
        }

        public string TokenEndpoint
        {
            get => AppSettings.GetValueOrDefault(IdTokenEndpoint, TokenEndpointDefault);
            set => AppSettings.AddOrUpdateValue(IdTokenEndpoint, value);
        }

        public string LogoutEndpoint
        {
            get => AppSettings.GetValueOrDefault(IdLogoutEndpoint, LogoutEndpointDefault);
            set => AppSettings.AddOrUpdateValue(IdLogoutEndpoint, value);
        }

        public string IdentityCallback
        {
            get => AppSettings.GetValueOrDefault(IdIdentityCallback, IdentityCallbackDefault);
            set => AppSettings.AddOrUpdateValue(IdIdentityCallback, value);
        }

        public string LogoutCallback
        {
            get => AppSettings.GetValueOrDefault(IdLogoutCallback, LogoutCallbackDefault);
            set => AppSettings.AddOrUpdateValue(IdLogoutCallback, value);
        }

        public string RevocationEndpoint
        {
            get => AppSettings.GetValueOrDefault(IdRevocationEndpoint, RevocationEndpointDefault);
            set => AppSettings.AddOrUpdateValue(IdRevocationEndpoint, value);
        }

        private void UpdateEndpoint(string avatarEndpoint)
        {
            // http://0.0.0.0/example/
            if (Uri.TryCreate(avatarEndpoint, UriKind.Absolute, out Uri avatarUri))
            {
                var authority = avatarUri.GetLeftPart(UriPartial.Authority);
                RegisterWebsite = $"{authority}/Account/Register";
                IdentityEndpoint = $"{authority}/connect/authorize";
                TokenEndpoint = $"{authority}/connect/token";
                LogoutEndpoint = $"{authority}/connect/endsession";
                IdentityCallback = $"{authority}/cortex/diary/callback";
                LogoutCallback = $"{authority}/Account/Redirecting";
                RevocationEndpoint = $"{authority}/connect/revocation";
                // DEL: LocationEndpoint = $"{authority}:5109";
            }
        }

        public void Clear()
        {
            this.AppSettings.Remove(IdAccessToken);
            this.AppSettings.Remove(IdToken);
            this.AppSettings.Remove(IdUseMocks);
            this.AppSettings.Remove(IdUseFakeLocation);
            this.AppSettings.Remove(IdLatitude);
            this.AppSettings.Remove(IdLongitude);
            this.AppSettings.Remove(IdAllowGpsLocation);
            this.AppSettings.Remove(IdBaseEndpoint);
            this.AppSettings.Remove(IdClientId);
            this.AppSettings.Remove(IdClientSecret);
            this.AppSettings.Remove(IdAuthToken);
            this.AppSettings.Remove(IdRegisterWebsite);
            this.AppSettings.Remove(IdentityEndpoint);
            this.AppSettings.Remove(IdLocationEndpoint);
            this.AppSettings.Remove(IdTokenEndpoint);
            this.AppSettings.Remove(IdLogoutEndpoint);
            this.AppSettings.Remove(IdentityCallback);
            this.AppSettings.Remove(IdLogoutCallback);
        }

        public void Update(string avatarUrl)
        {
            // split avatarUrl between server and avatar
            var uri = new Uri(avatarUrl);
            var server = uri.GetLeftPart(System.UriPartial.Authority);
        }
    }
}