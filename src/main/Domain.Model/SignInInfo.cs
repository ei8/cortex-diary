using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Domain.Model
{
    public class SignInInfo
    {
        public const string AnonymousGivenName = "Anonymous";

        public SignInInfo()
        {
            this.IdentityServer = new IdentityServerInfo();
            this.AuthAccessToken = string.Empty;
            this.AuthIdToken = string.Empty;
            this.GivenName = string.Empty;
            this.FamilyName = string.Empty;
            this.Email = string.Empty;
        }

        public IdentityServerInfo IdentityServer
        {
            get;
            set;
        }

        // DEL:
        public string AuthAccessToken
        {
            get;
            set;
        }

        public string AuthIdToken
        {
            get;
            set;
        }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }
    }
}
