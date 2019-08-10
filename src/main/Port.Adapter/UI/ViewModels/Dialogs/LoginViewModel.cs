using IdentityModel.Client;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Dialog;
using works.ei8.Cortex.Diary.Application.Identity;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.OpenUrl;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    public class LoginViewModel : DialogViewModelBase
    {
        private string _userName;
        private string _password;
        private bool _isMock;
        private bool _isValid;
        private bool _isLogin;
        private string _authUrl;

        private ISettingsService settingsService;
        private IOpenUrlService _openUrlService;
        private IIdentityService _identityService;

        public LoginViewModel(
            ISettingsService settingsService = null,
            IOpenUrlService openUrlService = null,
            IIdentityService identityService = null
            ) : base("Authentication")
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            this._openUrlService = openUrlService ?? Locator.Current.GetService<IOpenUrlService>();
            this._identityService = identityService ?? Locator.Current.GetService<IIdentityService>();

            _userName = string.Empty;
            _password = string.Empty;

            this.UserDialogResult = false;

            this.WhenAnyValue(x => x.IdentityServerUrl)
                .Subscribe(s => this.settingsService.IdentityServerUrl = s);

            // TODO: this.MockSignInCommand = ReactiveCommand.Create(async () => await MockSignInAsync());
            this.SignInCommand = ReactiveCommand.Create(async () => await SignInAsync());
            this.RegisterCommand = ReactiveCommand.Create(Register);
            this.NavigateCommand = ReactiveCommand.Create<string>(async (url) => await NavigateAsync(url));
            //this.SettingsCommand = ReactiveCommand.Create(async () => await SettingsAsync());
            //this.ValidateUserNameCommand = ReactiveCommand.Create(() => ValidateUserName());
            //this.ValidatePasswordCommand = ReactiveCommand.Create(() => ValidatePassword());

            // InvalidateMock();
            // TODO: AddValidations();
        }

        public string UserName
        {
            get => this._userName;
            set => this.RaiseAndSetIfChanged(ref this._userName, value);
        }

        public string Password
        {
            get => this._password;
            set => this.RaiseAndSetIfChanged(ref this._password, value);
        }

        public bool IsMock
        {
            get => this._isMock;
            set => this.RaiseAndSetIfChanged(ref this._isMock, value);
        }

        public bool IsValid
        {
            get => this._isValid;
            set => this.RaiseAndSetIfChanged(ref this._isValid, value);
        }

        public bool IsLogin
        {
            get => this._isLogin;
            set => this.RaiseAndSetIfChanged(ref this._isLogin, value);
        }

        public string LoginUrl
        {
            get => this._authUrl;
            set => this.RaiseAndSetIfChanged(ref this._authUrl, value);
        }

        private string identityServerUrl;

        public string IdentityServerUrl
        {
            get => this.identityServerUrl;
            set => this.RaiseAndSetIfChanged(ref this.identityServerUrl, value);
        }

        public ReactiveCommand MockSignInCommand { get; } 

        public ReactiveCommand SignInCommand { get; }

        public ReactiveCommand RegisterCommand { get; }

        public ReactiveCommand NavigateCommand { get; }

        public ReactiveCommand SettingsCommand { get; }

        public ReactiveCommand ValidateUserNameCommand { get; }

        public ReactiveCommand ValidatePasswordCommand { get; }

        // TODO: private async Task MockSignInAsync()
        //{
        //    IsValid = true;
        //    bool isValid = Validate();
        //    bool isAuthenticated = false;

        //    if (isValid)
        //    {
        //        try
        //        {
        //            await Task.Delay(1000);

        //            isAuthenticated = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"[SignIn] Error signing in: {ex}");
        //        }
        //    }
        //    else
        //    {
        //        IsValid = false;
        //    }

        //    if (isAuthenticated)
        //    {
        //        await NavigationService.NavigateToAsync<MainViewModel>();
        //        await NavigationService.RemoveLastFromBackStackAsync();
        //    }

        //    IsBusy = false;
        //}

        private async Task SignInAsync()
        {
            // TODO: IsBusy = true;

            var ss = (SettingsService) this.settingsService;
            ss.ClientId = Constants.ClientId;

            this.LoginUrl = _identityService.CreateAuthorizationRequest();

            IsValid = true;
            IsLogin = true;
            // TODO: IsBusy = false;
        }

        private void Register()
        {
            _openUrlService.OpenUrl(this.settingsService.RegisterWebsite);
        }

        private async Task NavigateAsync(string url)
        {
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.StartsWith(this.settingsService.LogoutCallback))
            {
                await this._identityService.RevokeAccessTokenAsync(this.settingsService.AuthAccessToken);
                this.settingsService.AuthAccessToken = string.Empty;
                this.settingsService.AuthIdToken = string.Empty;
                IsLogin = false;

                this.LoginUrl = _identityService.CreateAuthorizationRequest();
            }
            else if (unescapedUrl.Contains(this.settingsService.IdentityCallback))
            {
                var authResponse = new AuthorizeResponse(url);
                if (!string.IsNullOrWhiteSpace(authResponse.Code))
                {
                    var userToken = await _identityService.GetTokenAsync(authResponse.Code);
                    string accessToken = userToken.AccessToken;

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        this.settingsService.AuthAccessToken = accessToken;
                        this.settingsService.AuthIdToken = authResponse.IdentityToken;
                        this.UserDialogResult = true;
                        this.DialogResult = true;
                    }
                }
            }
        }

        // TODO: Necessary?
        //private bool Validate()
        //{
        //    bool isValidUser = ValidateUserName();
        //    bool isValidPassword = ValidatePassword();

        //    return isValidUser && isValidPassword;
        //}

        //private bool ValidateUserName()
        //{
        //    return _userName.Validate();
        //}

        //private bool ValidatePassword()
        //{
        //    return _password.Validate();
        //}

        //private void AddValidations()
        //{
        //    _userName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A username is required." });
        //    _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required." });
        //}

        //public void InvalidateMock()
        //{
        //    IsMock = this.settingsService.UseMocks;
        //}
    }
}