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

        public AddLabel(MainWindow mainWindow, string labelText = "")
        {
            MainWindow = mainWindow;
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(labelText))
            {
                Text.Text = labelText;
            }

            this.Topmost = true;
            this.Activate();

        }

        private bool CanClose()
        {
            var ok = true;

            if (string.IsNullOrWhiteSpace(Id.Text))
                ok = Helper.CheckFailed(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title);

            if (string.IsNullOrWhiteSpace(Text.Text))
                ok = Helper.CheckFailed(General.Validate_Add_EmptyText_Message, General.Validate_Add_EmptyText_Title);

            if (MainWindow.Labels.IdExists(Id.Text))
                ok = Helper.CheckFailed(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title);

            if (Id.Text.Contains("="))
                ok = Helper.CheckFailed(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title);

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
