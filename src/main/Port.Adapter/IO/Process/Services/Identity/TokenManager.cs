using IdentityModel.Client;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public class TokenManager
    {
        private readonly TokenProvider _tokenProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenManager(TokenProvider tokenProvider,
            IHttpClientFactory httpClientFactory)
        {
            _tokenProvider = tokenProvider ??
                throw new ArgumentNullException(nameof(tokenProvider));
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
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

            var discoveryReponse = await idpClient
                // TODO: make configurable
                .GetDiscoveryDocumentAsync("https://192.168.1.110");

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
               new RefreshTokenRequest
               {
                   Address = discoveryReponse.TokenEndpoint,
                   ClientId = Constants.ClientId,
                   ClientSecret = "978c1052-1184-48f7-89f4-4fd034847a06".Sha256(),
                   RefreshToken = _tokenProvider.RefreshToken
               });

            _tokenProvider.AccessToken = refreshResponse.AccessToken;
            _tokenProvider.RefreshToken = refreshResponse.RefreshToken;
            _tokenProvider.ExpiresAt = DateTime.UtcNow.AddSeconds(refreshResponse.ExpiresIn);

            return _tokenProvider.AccessToken;
        }
    }
}
