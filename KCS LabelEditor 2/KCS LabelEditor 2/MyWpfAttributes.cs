using System;
using System.Windows.Controls;

namespace KCS_LabelEditor_2
{
    public class MyWpfAttributes: Attribute
    {
        public double Width { get; set; }
        public DataGridLengthUnitType WidthType { get; set; } = DataGridLengthUnitType.Pixel;
        public string DisplayName { get; set; }
        public bool IsReadOnly { get; set; } = false;
    }
}