using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public struct Dendrite
    {
        public Dendrite(string id, string data, int version)
        {
            this.Id = id;
            this.Data = data;
            this.Version = version;
        }

        public string Id { get; set; }

        public string Data { get; set; }

        public int Version { get; set; }
    }
}
