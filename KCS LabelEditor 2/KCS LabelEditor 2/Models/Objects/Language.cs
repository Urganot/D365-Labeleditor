using System;
using System.Runtime.Serialization;

namespace AVA_LabelEditor.Objects
{
    [DataContract]
    public class Language : IEquatable<Language>
    {
        public Language(string name)
        {
            Name = name;
        }

        [DataMember]
        public string Name { get;protected set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Language other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Language)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}