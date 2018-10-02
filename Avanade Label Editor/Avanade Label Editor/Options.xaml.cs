using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AVA_LabelEditor
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private MainWindow _mainWindow;

        /// <summary>
        /// Constructor for Options window
        /// </summary>
        /// <param name="mainWindow">An instance of MainWindow</param>
        public Options(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            DataContext = _mainWindow;
        }

        private void PathTextbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mainWindow.SetPath())
                _mainWindow.ReloadLabels(true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
