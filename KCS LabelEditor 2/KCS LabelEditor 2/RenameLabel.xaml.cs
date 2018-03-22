using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using KCS_LabelEditor_2.Properties;

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

            DataContext = this;
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
                MessageBox.Show(General.Validate_Id_Empty_Message,General.Validate_Id_Empty_Title, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (NewIdTextbox.Text == OldIdTextbox.Text)
            {
                MessageBox.Show(General.Validate_Rename_OldEqualsNew_Message,General.Validate_Rename_OldEqualsNew_Title, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                ok = false;
            }

            if (_mainWindow.Labels.IdExists(NewIdTextbox.Text))
            {
                MessageBox.Show(General.Validate_Id_Exists_Message,General.Validate_Id_Exists_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            if (NewIdTextbox.Text.Contains("="))
            {
                MessageBox.Show(General.Validate_Id_InvalidCharacter_Message,General.Validate_Id_InvalidCharacter_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ok = false;
            }

            return ok;
        }
    }
}
