﻿using System;
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
        public const string DatabasePath = "DATABASE_PATH";
        public const string BasePath = "BASE_PATH";
        public const string PluginsPath = "PLUGINS_PATH";
        public const string ValidateServerCertificate = "VALIDATE_SERVER_CERTIFICATE";
        public const string AppTitle = "APP_TITLE";
        public const string AppIcon = "APP_ICON";
    }
}
