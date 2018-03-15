using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace KCS_LabelEditor_2
{
    public class Language : IEquatable<Language>
    {
        public string Name { get; set; }

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
            return Equals((Language) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}