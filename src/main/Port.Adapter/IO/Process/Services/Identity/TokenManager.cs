using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Domain.Model;
using IdentityModel.Client;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public class TokenManager : ITokenManager
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISettingsService settingsService;

        public TokenManager(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory, ISettingsService settingsService)
        {
            _tokenProvider = tokenProvider ??
                throw new ArgumentNullException(nameof(tokenProvider));
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
            this.settingsService = settingsService;
        }

        public async Task<string> RetrieveAccessTokenAsync()
        {
            // should we refresh? 
            if ((_tokenProvider.ExpiresAt.AddSeconds(-60)).ToUniversalTime()
                    > DateTime.UtcNow)
            {
                // no need to refresh, return the access token
                return _tokenProvider.AccessToken;
            }

            // refresh
            var idpClient = _httpClientFactory.CreateClient();

            var discoveryReponse = await idpClient.GetDiscoveryDocumentAsync(this.settingsService.OidcAuthority);

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = discoveryReponse.TokenEndpoint,
                   ClientId = this.settingsService.ClientId,
                   ClientSecret = this.settingsService.ClientSecret,
                   RefreshToken = _tokenProvider.RefreshToken
               });

            _tokenProvider.AccessToken = refreshResponse.AccessToken;
            _tokenProvider.RefreshToken = refreshResponse.RefreshToken;
            _tokenProvider.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn);

            return _tokenProvider.AccessToken;
        }
    }
}
