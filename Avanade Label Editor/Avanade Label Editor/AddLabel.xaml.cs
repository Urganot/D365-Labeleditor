using System.Windows;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Properties;

namespace AVA_LabelEditor
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
                ok = Helper.Helper.CheckFailed(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title);

            if (string.IsNullOrWhiteSpace(Text.Text))
                ok = Helper.Helper.CheckFailed(General.Validate_Add_EmptyText_Message, General.Validate_Add_EmptyText_Title);

            if (MainWindow.Labels.IdExists(Id.Text))
                ok = Helper.Helper.CheckFailed(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title);

            if (Id.Text.Contains("="))
                ok = Helper.Helper.CheckFailed(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title);

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
