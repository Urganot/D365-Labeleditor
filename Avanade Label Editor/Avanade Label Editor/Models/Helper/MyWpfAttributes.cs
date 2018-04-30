using System;
using System.Windows;
using System.Windows.Controls;

namespace AVA_LabelEditor.Helper
{
    public class MyWpfAttributes : Attribute
    {
        public double Width { get; set; }
        public DataGridLengthUnitType WidthType { get; set; } = DataGridLengthUnitType.Auto;
        public string DisplayName { get; set; }
        public bool IsReadOnly { get; set; } = false;
        public Visibility Visible { get; set; } = Visibility.Visible;
    }
}