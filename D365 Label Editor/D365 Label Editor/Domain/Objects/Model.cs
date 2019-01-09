using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AVA_LabelEditor.Objects
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class Model
    {
        private string _name;

        public string ItemName => Locked ? _name + " 🔒" : _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Path { get; set; }
        public string LabelPath { get; set; }
        public bool Locked { get; set; }

        public override string ToString()
        {
            return ItemName;
        }

        public bool Equals(Model other)
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
            return Equals((Model)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }

}
