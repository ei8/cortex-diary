using ei8.Cortex.Library.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Common
{
    public class UINeuron : Neuron, IEquatable<UINeuron>
    {
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
