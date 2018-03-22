using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for AddLabel.xaml
    /// </summary>
    public partial class AddLabel
    {
        private MainWindow MainWindow;

        public AddLabel(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        private bool CanClose()
        {
            var ok = true;
            if (string.IsNullOrWhiteSpace(Id.Text))
            {
                MessageBox.Show(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (string.IsNullOrWhiteSpace(Text.Text))
            {
                MessageBox.Show(General.Validate_Add_EmptyText_Message,General.Validate_Add_EmptyText_Title, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (MainWindow.Labels.IdExists(Id.Text))
            {
                MessageBox.Show(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            if (Id.Text.Contains("="))
            {
                MessageBox.Show(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            return ok;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            if (!CanClose())
            {
                return;
            }

            DialogResult = true;
            Close();
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
    }
}
