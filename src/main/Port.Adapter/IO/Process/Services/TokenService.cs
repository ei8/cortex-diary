using neurUL.Common.Http;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using ei8.Cortex.Diary.Application.Settings;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services
{
    public class TokenService : ITokenService
    {
        private readonly ISettingsService settingsService;

        public TokenService(ISettingsService settingsService = null)
        {
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public string GetAccessToken() => this.settingsService.AuthAccessToken;
    }
}
