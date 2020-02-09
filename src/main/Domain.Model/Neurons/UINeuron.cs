using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Graph.Client;

namespace works.ei8.Cortex.Diary.Domain.Model.Neurons
{
    // TODO: transfer to (cortex diary nucleus client) / (cortex diary nucleus common - if populated in nucleus itself, to prevent diary from referencing nucleus)?
    public class UINeuron : Neuron, IEquatable<UINeuron>
    {
        public UINeuron()
        {
        }

        public UINeuron(Neuron original)
        {
            this.Id = original.Id;
            this.Tag = original.Tag;
            this.Terminal = new Terminal()
            {
                AuthorId = original.Terminal.AuthorId,
                AuthorTag = original.Terminal.AuthorTag,
                Effect = original.Terminal.Effect,
                Id = original.Terminal.Id,
                PostsynapticNeuronId = original.Terminal.PostsynapticNeuronId,
                PresynapticNeuronId = original.Terminal.PresynapticNeuronId,
                Strength = original.Terminal.Strength,
                Timestamp = original.Terminal.Timestamp,
                Version = original.Terminal.Version
            };
            this.Version = original.Version;
            this.AuthorId = original.AuthorId;
            this.AuthorTag = original.AuthorTag;
            this.LayerId = original.LayerId;
            this.LayerTag = original.LayerTag;
            this.Timestamp = original.Timestamp;
            this.Errors = original.Errors;
        }

        [JsonIgnore]
        public int UIId { get; set; }

        [JsonIgnore]
        public int CentralUIId { get; set; }

        public override int GetHashCode()
        {
            return this.UIId.GetHashCode();
        }

        public bool Equals(UINeuron other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return UIId == other.UIId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UINeuron)obj);
        }

        public static bool operator ==(UINeuron left, UINeuron right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UINeuron left, UINeuron right)
        {
            return !Equals(left, right);
        }
    }
}
