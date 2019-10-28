using IdentityModel.Client;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Identity;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public class Helper
    {
        public static async Task<ProcessUrlResult> TryProcessUrl(string url, string identityCallbackPath, string logoutCallbackPath, string accessToken, IIdentityService identityService)
        {
            var result = ProcessUrlResult.Empty;
            var unescapedUrl = System.Net.WebUtility.UrlDecode(url);

            if (unescapedUrl.StartsWith(logoutCallbackPath))
            {
                await identityService.RevokeAccessTokenAsync(accessToken);
                result = new ProcessUrlResult(true, string.Empty, string.Empty, ProcessUrlType.Logout);
            }
            else if(unescapedUrl.Contains(identityCallbackPath))
            {
                var authResponse = new AuthorizeResponse(unescapedUrl);
                if (!string.IsNullOrWhiteSpace(authResponse.Code))
                {
                    var userToken = await identityService.GetTokenAsync(authResponse.Code);
                    if (!string.IsNullOrWhiteSpace(userToken.AccessToken))
                    {
                        result = new ProcessUrlResult(true, userToken.AccessToken, authResponse.IdentityToken, ProcessUrlType.SignIn);
                    }
                }
            }

            return result;
        }
    }
}
