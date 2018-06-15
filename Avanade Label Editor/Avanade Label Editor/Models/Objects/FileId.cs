using System;
using System.Runtime.Serialization;

namespace AVA_LabelEditor.Objects
{
    /// <summary>
    /// This class represents a FileId
    /// </summary>
    [DataContract]
    public class FileId : IEquatable<FileId>
    {
        /// <summary>
        /// Name of the FileId
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(FileId other)
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
            return Equals((FileId) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
