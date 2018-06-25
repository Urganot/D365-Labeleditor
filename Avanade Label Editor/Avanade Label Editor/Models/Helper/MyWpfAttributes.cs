using System;
using System.Windows;
using System.Windows.Controls;

namespace AVA_LabelEditor.Helper
{
    /// <summary>
    /// Custom attributes for column generation
    /// </summary>
    public class MyWpfAttributes : Attribute
    {
        /// <summary>
        /// Width of the column
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Width type
        /// </summary>
        public DataGridLengthUnitType WidthType { get; set; } = DataGridLengthUnitType.Auto;

        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Is column read only
        /// </summary>
        public bool IsReadOnly { get; set; } = false;

        /// <summary>
        /// Visibility
        /// </summary>
        public bool Visible { get; set; } = true;
    }
}