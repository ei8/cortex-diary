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

using IdentityModel;
using IdentityModel.Client;
using neurUL.Common.Http;
using PCLCrypto;
using Splat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Helpers;
using static PCLCrypto.WinRTCrypto;
using ei8.Cortex.Diary.Domain.Model;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ISettingsService settingsService;
        private readonly IRequestProvider requestProvider;
        private string _codeVerifier;

        public IdentityService(ISettingsService settingsService = null, IRequestProvider requestProvider = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }

        public string CreateAuthorizationRequest(string authorizeEndpoint)
        {
            // Create URI to authorization endpoint
            var authorizeRequest = new AuthorizeRequest(authorizeEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", this.settingsService.ClientId);
            dic.Add("client_secret", this.settingsService.ClientSecret);
            dic.Add("response_type", "code id_token");
            dic.Add("scope", "openid profile offline_access avatar"); 
            dic.Add("redirect_uri", this.settingsService.LoginCallback);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));
            //TODO: required if PKCE in server is true
            //dic.Add("code_challenge", CreateCodeChallenge());
            //dic.Add("code_challenge_method", "S256");

            // TODO: Add CSRF token to protect against cross-site request forgery attacks.
            //var currentCSRFToken = Guid.NewGuid().ToString("N");
            //dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);
            return authorizeUri;
        }

        // TODO: Navigate here to initiate sign-out
        public string CreateLogoutRequest(string idToken, string identityServerUrl)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                return string.Empty;
            }

            var ru = new RequestUrl(identityServerUrl);
            var es = ru.CreateEndSessionUrl(
                idToken,
                this.settingsService.LogoutCallback,
                idToken
                );
            return es;
            // TODO: use something other than bearerToken
            // DEL:return $"{identityServerInfo.LogoutEndpoint}?id_token_hint={bearerToken}&post_logout_redirect_uri={WebUtility.UrlEncode(identityServerInfo.LogoutCallback)}&state={bearerToken}";
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string bearerToken, string userInfoEndpoint)
        {
            return await this.requestProvider.HttpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = userInfoEndpoint,
                Token = bearerToken
            });
        }

        public async Task<TokenResponse> GetTokenAsync(string code, string tokenEndpoint)
        {
            return await this.requestProvider.HttpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = tokenEndpoint,
                ClientId = this.settingsService.ClientId,
                ClientSecret = this.settingsService.ClientSecret,
                Code = code,
                RedirectUri = this.settingsService.LoginCallback,
                CodeVerifier = _codeVerifier
            });
        }

        public async Task<TokenRevocationResponse> RevokeAccessTokenAsync(string bearerToken, string revocationEndpoint)
        {
            return await this.requestProvider.HttpClient.RevokeTokenAsync(new TokenRevocationRequest {
                Address = revocationEndpoint,
                ClientId = this.settingsService.ClientId,
                ClientSecret = this.settingsService.ClientSecret
            });
        }

        private string CreateCodeChallenge()
        {
            _codeVerifier = RandomNumberGenerator.CreateUniqueId();
            var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(_codeVerifier)));
            byte[] challengeBytes;
            CryptographicBuffer.CopyToByteArray(challengeBuffer, out challengeBytes);
            return Base64Url.Encode(challengeBytes);
        }
    }
}
