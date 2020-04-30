using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity
{
    public struct ProcessUrlResult
    {
        public ProcessUrlResult(bool success, string accessToken, string identityToken, ProcessUrlType type)
        {
            this.Success = success;
            this.AccessToken = accessToken;
            this.IdentityToken = identityToken;
            this.Type = type;
        }
        public bool Success { get; private set; }
        public string AccessToken { get; private set; }
        public string IdentityToken { get; private set; }
        public ProcessUrlType Type { get; private set; }

        public static readonly ProcessUrlResult Empty = new ProcessUrlResult()
        {
            Success = false,
            AccessToken = string.Empty,
            IdentityToken = string.Empty,
            Type = ProcessUrlType.NotSet    
        };
    }

    public enum ProcessUrlType
    {
        NotSet,
        Logout,
        SignIn
    }
}
