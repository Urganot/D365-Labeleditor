using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Properties;
using static AVA_LabelEditor.Helper.Helper;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for AddLabel.xaml
    /// </summary>
    public partial class AddLabel
    {
        private AVA_LabelEditor.MainWindow MainWindow;

        public AddLabel(AVA_LabelEditor.MainWindow mainWindow, string labelText = "")
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
                ok = CheckFailed(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title);

            if (string.IsNullOrWhiteSpace(Text.Text))
                ok = CheckFailed(General.Validate_Add_EmptyText_Message, General.Validate_Add_EmptyText_Title);

            if (MainWindow.Labels.IdExists(Id.Text))
                ok = CheckFailed(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title);

            if (Id.Text.Contains("="))
                ok = CheckFailed(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title);

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowWithoutCommandbar.RemoveCommandBar(this);
        }
    }
}
