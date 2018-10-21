using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronDto : IEquatable<NeuronDto>
    {
        public NeuronDto(int id, int parentId, string neuronId, string parentNeuronId, string data, ChildType type)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.NeuronId = neuronId;
            this.ParentNeuronId = parentNeuronId;
            this.Data = data;
            this.Type = type;
        }

        public int Id { get; }
        public int ParentId { get; }
        public string NeuronId { get; }
        public string ParentNeuronId { get; }
        public string Data { get; }
        public ChildType Type { get; }

        public bool Equals(NeuronDto other)
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
            return Equals((NeuronDto)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(NeuronDto left, NeuronDto right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NeuronDto left, NeuronDto right)
        {
            return !Equals(left, right);
        }
    }
}
