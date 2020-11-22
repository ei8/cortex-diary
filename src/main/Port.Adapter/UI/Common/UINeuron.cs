using ei8.Cortex.Library.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Common
{
    public class UINeuron : NeuronResult, IEquatable<UINeuron>
    {
        public UINeuron()
        {
        }

        public UINeuron(UINeuron original) : this((NeuronResult) original)
        {
            this.UIId = original.UIId;
            this.CentralUIId = original.CentralUIId;
        }

        public UINeuron(NeuronResult original) : base(original)
        {
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

        public void CopyData(UINeuron original)
        {
            this.Id = original.Id;
            this.Tag = original.Tag;
            this.Creation = new Library.Common.AuthorEventInfo(original.Creation);
            this.LastModification = new Library.Common.AuthorEventInfo(original.LastModification);
            this.UnifiedLastModification = new Library.Common.AuthorEventInfo(original.UnifiedLastModification);
            this.Region = new Library.Common.NeuronInfo(original.Region);
            this.Version = original.Version;
            this.Active = original.Active;
            this.ReadOnly = original.ReadOnly;
            this.restrictionReasons = new List<string>(original.restrictionReasons);

            if (original.Terminal != null)
                this.Terminal = new Terminal(original.Terminal);
        }
    }
}
