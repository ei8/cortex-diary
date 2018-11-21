using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    public class Neuron : IEquatable<Neuron>
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public int CentralId { get; set; }

        [JsonProperty("Id")]
        public string NeuronId { get; set; }

        [JsonProperty("CentralId")]
        public string CentralNeuronId { get; set; }

        public string Tag { get; set; }

        public RelativeType Type { get; set; }

        public int Version { get; set; }

        public string Timestamp { get; set; }

        public IList<string> Errors { get; set; }

        public bool Equals(Neuron other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Neuron)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Neuron left, Neuron right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Neuron left, Neuron right)
        {
            return !Equals(left, right);
        }
    }
}
