using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Pages
{
    public class LoginIdPModel : PageModel
    {
        public async Task OnGetAsync(string redirectUri)
        {
            if (string.IsNullOrWhiteSpace(redirectUri))
            {
                redirectUri = Url.Content("~/");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(redirectUri);
            }

            await HttpContext.ChallengeAsync(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        }
    }
}