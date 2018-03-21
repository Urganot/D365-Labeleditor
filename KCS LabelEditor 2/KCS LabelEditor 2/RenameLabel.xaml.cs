using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for RenameLabel.xaml
    /// </summary>
    public partial class RenameLabel
    {
        private MainWindow _mainWindow;
        public RenameLabel(MainWindow mainWindow,string oldId)
        {

            _mainWindow = mainWindow;
            InitializeComponent();

            OldIdTextbox.Text = oldId;
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            DialogResult = true;
            Close();

        }

        private bool ValidateForm()
        {
            var ok = true;
            if (string.IsNullOrWhiteSpace(NewIdTextbox.Text))
            {
                MessageBox.Show("Id darf nicht leer sein", "Id darf nicht leer sein", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (NewIdTextbox.Text == OldIdTextbox.Text)
            {
                MessageBox.Show("Neue Id darf nicht gleich der alten sein.", "Neue Id darf nicht gleich der alten sein.", MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (_mainWindow.Labels.IdExists(NewIdTextbox.Text))
            {
                MessageBox.Show("Id ist bereits vorhanden.", "Id gefunden", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            if (NewIdTextbox.Text.Contains("="))
            {
                MessageBox.Show("Id beinhaltet das ungültige Zeichen \"=\".", "Ungültiges Zeichen", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            return ok;
        }
    }
}
