using ei8.Cortex.Library.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Common
{
    public static class ExtensionMethods
    {
        public static UINeuron ToInternalType(this Neuron value)
        {
            return new UINeuron()
            {
                Active = value.Active,
                AuthorId = value.AuthorId,
                AuthorTag = value.AuthorTag,
                Id = value.Id,
                RegionId = value.RegionId,
                RegionTag = value.RegionTag,
                Tag = value.Tag,
                Terminal = value.Terminal != null ? new Terminal(value.Terminal) : null,
                Timestamp = value.Timestamp,
                Version = value.Version
            };
        }
    }
}
