using System.Windows;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Properties;

namespace AVA_LabelEditor
{
    /// <summary>
    /// Interaction logic for RenameLabel.xaml
    /// </summary>
    public partial class RenameLabel
    {

        private MainWindow _mainWindow;

        /// <summary>
        /// Constructor for RenameLabelWindow
        /// </summary>
        /// <param name="mainWindow">An instance of MainWindow</param>
        /// <param name="oldId">The labelId that should be changed</param>
        public RenameLabel(MainWindow mainWindow, string oldId)
        {

            _mainWindow = mainWindow;
            InitializeComponent();

            OldIdTextbox.Text = oldId;

            DataContext = this;
        }

        /// <summary>
        /// Handles Window loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NativeMethods.RemoveCommandBar(this);
        }

        /// <summary>
        /// Handles Ok-BUtton click
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
        /// Validates Inputs on window
        /// </summary>
        /// <returns>True is inputs are valid</returns>
        private bool CanClose()
        {
            var ok = true;

            if (string.IsNullOrWhiteSpace(NewIdTextbox.Text))
                ok = Helper.Helper.CheckFailed(General.Validate_Id_Empty_Message, General.Validate_Id_Empty_Title);

            if (NewIdTextbox.Text == OldIdTextbox.Text)
                ok = Helper.Helper.CheckFailed(General.Validate_Rename_OldEqualsNew_Message, General.Validate_Rename_OldEqualsNew_Title);

            if (_mainWindow.Labels.IdExists(NewIdTextbox.Text))
                ok = Helper.Helper.CheckFailed(General.Validate_Id_Exists_Message, General.Validate_Id_Exists_Title);

            if (NewIdTextbox.Text.Contains("="))
                ok = Helper.Helper.CheckFailed(General.Validate_Id_InvalidCharacter_Message, General.Validate_Id_InvalidCharacter_Title);

            return ok;
        }
    }
}
