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
        public const string ClientId = "CLIENT_ID";
        public const string ClientSecret = "CLIENT_SECRET";
        public const string UpdateCheckInterval = "UPDATE_CHECK_INTERVAL";
        public const string DatabasePath = "DATABASE_PATH";
        public const string BasePath = "BASE_PATH";
        public const string ValidateServerCertificate = "VALIDATE_SERVER_CERTIFICATE";
        public const string APP_TITLE = "APP_TITLE";
        public const string APP_ICON = "APP_ICON";
    }
}
