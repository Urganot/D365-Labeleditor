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

        /// <summary>
        /// Constructor for the AddLabel window
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow</param>
        /// <param name="labelText">(optional) A string to preset the text input</param>
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

        /// <summary>
        /// Validates the windows inputs
        /// </summary>
        /// <returns>True is inputs are valid</returns>
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

        /// <summary>
        /// Handles the ok button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

            if (!CanClose())
            {
                return;
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the window loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowWithoutCommandbar.RemoveCommandBar(this);
        }
    }
}
