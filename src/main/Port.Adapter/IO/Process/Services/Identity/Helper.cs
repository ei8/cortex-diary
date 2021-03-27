using IdentityModel.Client;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Domain.Model;
using System.Linq;
using neurUL.Common.Http;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public class Helper
    {
        public static async Task<ProcessUrlResult> TryProcessLoginUrl(string url, string loginCallbackPath, IIdentityService identityService, string tokenEndpoint)
        {
            var result = ProcessUrlResult.Empty;
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if(unescapedUrl.Contains(loginCallbackPath))
            {
                var authResponse = new AuthorizeResponse(unescapedUrl);
                if (!string.IsNullOrWhiteSpace(authResponse.Code))
                {
                    var tokenResponse = await identityService.GetTokenAsync(authResponse.Code, tokenEndpoint);
                    if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                    {
                        result = new ProcessUrlResult(true, tokenResponse.AccessToken, authResponse.IdentityToken, ProcessUrlType.SignIn);
                    }
                }
            }

            return result;
        }

        public static async Task<ProcessUrlResult> TryProcessLogoutUrl(string url, string logoutCallbackPath, IIdentityService identityService, ISignInInfoService signInInfoService) //TODO: , IRequestProvider requestProvider)
        {
            var result = ProcessUrlResult.Empty;
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.Contains(logoutCallbackPath))
            {
                // TODO: get accessToken from state which should be equal to signininfo.id (currently equal to accesstoken), 
                // TODO: get identityServerInfo from service using state (signininfo.id)
                var signInInfo = signInInfoService.SignIns.Single(sii => sii.GivenName != "Anonymous");
                var identityServerInfo = new IdentityServerInfo();
                // TODO: requestProvider.HttpClient.GetDiscoveryDocumentAsync()
                var response = await identityService.RevokeAccessTokenAsync(signInInfo.AuthAccessToken, signInInfo.IdentityServer.RevocationEndpoint);
                result = new ProcessUrlResult(true, string.Empty, string.Empty, ProcessUrlType.Logout);
            }
            
            return result;
        }
    }
}
