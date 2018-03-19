using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public class Terminal
    {
        public string Id { get; set; }

        public string TargetId { get; set; }

        public string TargetData { get; set; }
    }
}
