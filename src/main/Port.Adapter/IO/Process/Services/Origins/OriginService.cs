using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Origin;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Origins
{
    public class OriginService : IOriginService
    {
        private readonly IOriginsCacheService originsCacheService;

        public OriginService(IOriginsCacheService originsCacheService = null)
        {
            this.originsCacheService = originsCacheService ?? Locator.Current.GetService<IOriginsCacheService>();
        }

        public Avatar GetAvatarByUrl(string url)
        {
            Avatar result = null;
            if (this.IsAvailableAndReachable(this.Home))
            {
                //TODO:
            }
            else
            {
                result = this.originsCacheService.GetAvatarByUrl(url);
            }
            return result;
        }

        public Avatar Home { get; set; }

        public Server GetServer(Avatar value)
        {
            throw new NotImplementedException();
        }

        public bool IsReachable(Avatar value)
        {
            // TODO:
            return false;
        }

        public void Save(string avatarUrl)
        {
            var uri = new Uri(avatarUrl);
            var serverUrl = uri.GetLeftPart(UriPartial.Authority);
            var avatarName = uri.GetLeftPart(UriPartial.Path).Substring(serverUrl.Length + 1);

            // if home Avatar is available and reachable
            if (this.IsAvailableAndReachable(this.Home))
            {
                // TODO: add/update in home
            }
            else
            {
                Server server = null;

                // otherwise, add in cache if not yet there
                if (!this.originsCacheService.ContainsServer(serverUrl))
                {
                    server = new Server
                    {
                        Id = Guid.NewGuid().ToString(),
                        Url = serverUrl
                    };
                    this.originsCacheService.Add(server);
                }
                else
                    server = this.originsCacheService.GetServerByUrl(serverUrl);

                if (!this.originsCacheService.ContainsAvatar(avatarUrl))
                {
                    this.originsCacheService.Add(new Avatar
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = avatarName,
                        ServerId = server.Id
                    });
                }
            }
        }

        private bool IsAvailableAndReachable(Avatar value)
        {
            return value != null && this.IsReachable(value);
        }
    }
}
