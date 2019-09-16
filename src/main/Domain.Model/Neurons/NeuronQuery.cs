using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public class NeuronQuery
    {
        public IEnumerable<string> Postsynaptic { get; set; }

        public IEnumerable<string> PostsynapticNot { get; set; }

        public IEnumerable<string> Presynaptic { get; set; }

        public IEnumerable<string> PresynapticNot { get; set; }

        public IEnumerable<string> TagContains { get; set; }

        public IEnumerable<string> TagContainsNot { get; set; }

        public IEnumerable<string> Id { get; set; }

        public IEnumerable<string> IdNot { get; set; }
    }
}
