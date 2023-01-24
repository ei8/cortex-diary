using IdentityModel.Client;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Dialog;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Navigation;
using ei8.Cortex.Diary.Application.OpenUrl;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    public class LoginViewModel : DialogViewModelBase
    {
        private string userName;
        private string password;
        private bool isMock;
        private bool isValid;
        private bool isLogin;
        private string authUrl;

        private ISettingsService settingsService;
        private IOpenUrlService openUrlService;
        private IIdentityService identityService;

        public LoginViewModel(
            ISettingsService settingsService = null,
            IOpenUrlService openUrlService = null,
            IIdentityService identityService = null
            ) : base("Authentication")
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            this.openUrlService = openUrlService ?? Locator.Current.GetService<IOpenUrlService>();
            this.identityService = identityService ?? Locator.Current.GetService<IIdentityService>();

            userName = string.Empty;
            password = string.Empty;

            this.UserDialogResult = false;

            this.WhenAnyValue(x => x.IdentityServerUrl)
                .Subscribe(s =>
                {
                    // TODO: this.settingsService.IdentityServerUrl = s;
                    // TODO: this.settingsService.ApplicationUrl = "http://192.168.1.110:59053"; // TEMP ONLY!!!
                });

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
            get => this.userName;
            set => this.RaiseAndSetIfChanged(ref this.userName, value);
        }

        public string Password
        {
            get => this.password;
            set => this.RaiseAndSetIfChanged(ref this.password, value);
        }

        public bool IsMock
        {
            get => this.isMock;
            set => this.RaiseAndSetIfChanged(ref this.isMock, value);
        }

        public bool IsValid
        {
            get => this.isValid;
            set => this.RaiseAndSetIfChanged(ref this.isValid, value);
        }

        public bool IsLogin
        {
            get => this.isLogin;
            set => this.RaiseAndSetIfChanged(ref this.isLogin, value);
        }

        [Reactive]
        public bool IsNavigating { get; set; }

        public string LoginUrl
        {
            get => this.authUrl;
            set => this.RaiseAndSetIfChanged(ref this.authUrl, value);
        }

        private string identityServerUrl;

        public string IdentityServerUrl
        {
            get => this.identityServerUrl;
            set => this.RaiseAndSetIfChanged(ref this.identityServerUrl, value);
        }

        public ReactiveCommand<Unit, Unit> MockSignInCommand { get; } 

        public ReactiveCommand<Unit, Task> SignInCommand { get; }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public ReactiveCommand<string, Unit> NavigateCommand { get; }

        public ReactiveCommand<Unit, Unit> SettingsCommand { get; }

        public ReactiveCommand<Unit, Unit> ValidateUserNameCommand { get; }

        public ReactiveCommand<Unit, Unit> ValidatePasswordCommand { get; }

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

            // TODO: this.LoginUrl = identityService.CreateAuthorizationRequest();

            IsValid = true;
            IsLogin = true;
            // TODO: IsBusy = false;
        }

        private void Register()
        {
            // TODO: openUrlService.OpenUrl(this.settingsService.RegisterWebsite);
        }

        private async Task NavigateAsync(string url)
        {
            this.IsNavigating = false;
            var result = ProcessUrlResult.Empty;
            
            // TODO: if ((result = (await IO.Process.Services.Identity.Helper.TryProcessUrl(
            //    url, 
            //    settingsService.IdentityCallback, 
            //    settingsService.LogoutCallback, 
            //    settingsService.AuthAccessToken, 
            //    identityService
            //    ))).Success)
            //{
            //    settingsService.AuthAccessToken = result.AccessToken;
            //    settingsService.AuthIdToken = result.IdentityToken;

            //    switch (result.Type)
            //    {
            //        case ProcessUrlType.Logout:
            //            this.IsLogin = false;
            //            this.LoginUrl = identityService.CreateAuthorizationRequest();
            //            break;
            //        case ProcessUrlType.SignIn:
            //            this.UserDialogResult = this.DialogResult = true;
            //            break;
            //    }                
            //}
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