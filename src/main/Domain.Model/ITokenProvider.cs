using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Domain.Model
{
    public interface ITokenProvider
    {
        string XsrfToken { get; set; }
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
        DateTimeOffset ExpiresAt { get; set; }
    }
}
