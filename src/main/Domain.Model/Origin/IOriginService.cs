using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Origin
{
    public interface IOriginService
    {
        // TODO: get servers and avatars data from "avatar/config" resource

        // neurons of home avatar etc (use INeuronClient and INeuronGraphQueryClient)
        // Generates and saves Servers and Avatars to (1) in-memory cache, and to (2) Home if it is available reachable.
        void Save(string avatarUrl);
        
        /// <summary>
        /// Uploads Origins data to home avatar.
        /// </summary>
        // TODO: bool UploadOfflineOriginsToHome();
        //
        //bool HasOfflineOrigins();
        //
        /// <summary>
        /// TODO: Home Avatar should be set once a user signs-in to Home successfully
        /// </summary>
        /// <returns></returns>
        Avatar Home { get; set; }

        bool IsReachable(Avatar value);

        /// <summary>
        /// Retrieves Owned Avatars from reachable avatars.
        /// </summary>
        /// <returns></returns>
        // IEnumerable<Avatar> GetOwnedAvatars();

        Server GetServer(Avatar value);

        Avatar GetAvatarByUrl(string url);
    }
}
