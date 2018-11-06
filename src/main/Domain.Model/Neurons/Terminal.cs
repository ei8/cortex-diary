using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public class Terminal
    {
        public string TargetId { get; set; }
        public NeurotransmitterEffect Effect { get; set; }
        public float Strength { get; set; }
    }
}
