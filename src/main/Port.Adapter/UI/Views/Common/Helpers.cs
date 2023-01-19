using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Common
{
    public static class Helpers
    {
        public static class String
        {
            public static bool IsExternalUrl(string currentUrlString, string avatarUrlString)
            {
                bool result = false;
                if (Uri.TryCreate(currentUrlString, UriKind.Absolute, out Uri currentUri) && Uri.TryCreate(avatarUrlString, UriKind.Absolute, out Uri avatarUri))
                    result = currentUri.Authority != avatarUri.Authority;

                return result;
            }
        }
    }
}
