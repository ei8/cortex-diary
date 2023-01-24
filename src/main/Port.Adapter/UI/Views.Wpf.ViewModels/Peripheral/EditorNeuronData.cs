using neurUL.Cortex.Common;
using System;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class EditorNeuronData : IEquatable<EditorNeuronData>
    {
        public EditorNeuronData(string id, string tag, NeurotransmitterEffect? effect, float? strength, RelativeType? relativeType, string regionId, string regionName, int version)
        {
            this.Id = id;
            this.Tag = tag;
            this.Effect = effect;
            this.Strength = strength;
            this.RelativeType = relativeType;
            this.RegionId = regionId;
            this.RegionName = regionName;
            this.Version = version;
        }

        public string Id { get; }

        public NeurotransmitterEffect? Effect { get; }

        public string RegionId { get; }

        public string RegionName { get; }

        public RelativeType? RelativeType { get; }

        public float? Strength { get; }

        public string Tag { get; }

        public int Version { get; }

        public bool Equals(EditorNeuronData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Id == other.Id &&
                this.Effect == other.Effect &&
                this.RegionId == other.RegionId &&
                this.RegionName == other.RegionName &&
                this.RelativeType == other.RelativeType &&
                this.Strength == other.Strength &&
                this.Tag == other.Tag &&
                this.Version == other.Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EditorNeuronData)obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(EditorNeuronData left, EditorNeuronData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EditorNeuronData left, EditorNeuronData right)
        {
            return !Equals(left, right);
        }
    }
}
