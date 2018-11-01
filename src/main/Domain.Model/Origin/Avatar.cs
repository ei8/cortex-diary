using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Origin
{
    public class Avatar
    {
        public string Id { get; set; }

        public string ServerId { get; set; }

        public string Name { get; set; }

        public bool IsHome { get; set; }
    }
}
