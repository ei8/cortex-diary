using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Origin
{
    /// <summary>
    /// Serves as an in-memory storage of servers and avatars that haven't been persisted in a home avatar. Once persisted, this should be cleared.
    /// </summary>
    public interface IOriginsCacheService
    {
        void Add(Avatar value, bool online);

        void Add(Server value, bool online);

        // TODO: void ClearOfflineData();

        //IEnumerable<Avatar> OfflineAvatars { get; }

        //IEnumerable<Server> OfflineServers { get; }
    }
}
