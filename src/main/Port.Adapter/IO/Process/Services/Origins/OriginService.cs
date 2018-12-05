/*
    This file is part of the d# project.
    Copyright (c) 2016-2018 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

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
