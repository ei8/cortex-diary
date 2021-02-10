using IdentityModel.Client;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Domain.Model;

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

        public static async Task<ProcessUrlResult> TryProcessLogoutUrl(string url, string logoutCallbackPath, IIdentityService identityService)
        {
            var result = ProcessUrlResult.Empty;
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.Contains(logoutCallbackPath))
            {
                // TODO: get accessToken from state which should be equal to signininfo.id (currently equal to accesstoken), 
                // TODO: get identityServerInfo from service using state (signininfo.id)
                var accessToken = string.Empty;
                var identityServerInfo = new IdentityServerInfo();
                var response = await identityService.RevokeAccessTokenAsync(accessToken, identityServerInfo.RevocationEndpoint);
                result = new ProcessUrlResult(true, string.Empty, string.Empty, ProcessUrlType.Logout);
            }
            
            return result;
        }
    }
}
