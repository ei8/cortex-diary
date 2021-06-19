using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.Common
{
    public enum EventTypeNames
    {
        NeuronCreated,
        NeuronTagChanged,
        TerminalCreated,
        TerminalDeactivated
    }

    public struct EnvironmentVariableKeys
    {
        public const string OidcAuthority = "OIDC_AUTHORITY";
        public const string ClientSecret = "CLIENT_SECRET";
        public const string UpdateCheckInterval = "UPDATE_CHECK_INTERVAL";
        public const string DatabasePath = "DATABASE_PATH";
    }
}
