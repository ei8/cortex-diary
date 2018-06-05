using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public class Neuron
    {
        public string Id { get; set; }

        public string Data { get; set; }

        public int Version { get; set; }

        public string Timestamp { get; set; }

        // Children (to outer/upper cortex layers)
        public IList<Dendrite> Dendrites { get; set; }

        // Parents (to inner/lower cortex layers)
        public IList<Terminal> Terminals { get; set; }

        public IList<string> Errors { get; set; }
    }
}
