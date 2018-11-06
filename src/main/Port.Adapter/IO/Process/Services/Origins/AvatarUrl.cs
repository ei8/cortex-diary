using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Origin;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Origins
{
    public class AvatarUrl
    {
        public AvatarUrl(Avatar avatar, string url)
        {
            this.Avatar = avatar;
            this.Url = url;
        }

        public Avatar Avatar { get; private set; }

        public string Url { get; private set; }
    }
}
