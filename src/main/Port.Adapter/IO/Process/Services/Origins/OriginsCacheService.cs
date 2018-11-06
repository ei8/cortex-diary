using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Origin;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Origins
{
    public class OriginsCacheService : IOriginsCacheService
    {
        private readonly Dictionary<string, Avatar> avatars;
        private readonly Dictionary<string, Server> servers;

        public OriginsCacheService()
        {
            this.avatars = new Dictionary<string, Avatar>();
            this.servers = new Dictionary<string, Server>();
        }

        public void Add(Avatar value)
        {
            this.avatars.Add(value.Id, value);
        }

        public void Add(Server value)
        {
            this.servers.Add(value.Id, value);
        }

        public bool ContainsAvatar(string avatarUrl)
        {
            IEnumerable<AvatarUrl> uas = this.GetAvatarUrls();

            return uas.Any(ua => ua.Url == avatarUrl);
        }

        private IEnumerable<AvatarUrl> GetAvatarUrls()
        {
            return from se in this.servers.Values
                   from av in this.avatars.Values
                   where se.Id == av.ServerId
                   select new AvatarUrl(av, $"{se.Url}/{av.Name}");
        }

        public bool ContainsServer(string serverUrl)
        {
            return this.servers.Values.Any(s => s.Url == serverUrl);
        }

        public Avatar GetAvatarByUrl(string avatarUrl)
        {
            return this.GetAvatarUrls().First(au => au.Url == avatarUrl).Avatar;
        }

        public Server GetServerByUrl(string serverUrl)
        {
            return this.servers.Values.First(s => s.Url == serverUrl);
        }
    }
}
