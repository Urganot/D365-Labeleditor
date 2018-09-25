using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using AVA_LabelEditor.CustomExceptions;
using AVA_LabelEditor.Domain.Helper;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Lists;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;
using DataGrid = System.Windows.Controls.DataGrid;
using Label = AVA_LabelEditor.Objects.Label;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AVA_LabelEditor
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for halloWelt.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Properties

        public Server.Server Server;

        public event PropertyChangedEventHandler PropertyChanged;

        public XmlFiles XmlFiles;
        public Models Models;
        public LabelFiles ReadFilesNew;
        public Labels Labels;
        public Languages Languages;
        public FileIds FileIds;

        /// <summary>
        /// String to search for
        /// </summary>
        public string SearchString
        {
            get
            {
                return SearchTextbox.Text;
            }
            set { SearchTextbox.Text = value; }
        }

        public BackgroundTimer Timer;

        /// <summary>
        /// Was something changed
        /// </summary>
        public bool Changed { get; set; }

        internal object GetWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Path to the AxLabelFile
        /// </summary>
        public string AxLabelPath
        {
            get { return Settings.Default.AxLabelPath; }
            set
            {
                Settings.Default.AxLabelPath = value;
                OnPropertyChanged();
            }

        }

        /// <summary>
        /// Is automatic translation enabled
        /// </summary>
        public bool AutoTranslate
        {
            get { return Settings.Default.AutoTranslate; }
            set
            {
                Settings.Default.AutoTranslate = value;
                OnPropertyChanged();
            }
        }


        #endregion
        /// <summary>
        /// Constructor for the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Init();

            SetDataBindings();

            if (string.IsNullOrWhiteSpace(AxLabelPath) || !Directory.Exists(AxLabelPath))
            {
                if (!SetPath())
                    return;
            }

            ReloadLabels(true);

            InitTimer();

            try
            {
                Server.Start();
            }
            catch (ConnectionAlreadyOpenException)
            { }
        }

        /// <summary>
        /// Inits the timer
        /// </summary>
        private void InitTimer()
        {
            Timer = new BackgroundTimer(this);
            Timer.Init();
            Timer.Start();
        }

        /// <summary>
        /// Inits the properties
        /// </summary>
        private void Init()
        {
            Models = new Models(this);
            Labels = new Labels(this);
            Languages = new Languages(this);
            FileIds = new FileIds(this);
            XmlFiles = new XmlFiles(this);
            ReadFilesNew = new LabelFiles(this);
            Server = new Server.Server(this);
        }

        /// <summary>
        /// Sets the Databinding on the WPF controls
        /// </summary>
        private void SetDataBindings()
        {
            DataContext = this;

            MainGrid.DataContext = Labels;
            MainGrid.ItemsSource = Labels.GetView();

            SubGrid.DataContext = Labels;
            SubGrid.ItemsSource = Labels.GetView();

            LanguageCombobox.DataContext = Languages;
            LanguageCombobox.ItemsSource = Languages.GetView();

            FileIdCombobox.DataContext = FileIds;
            FileIdCombobox.ItemsSource = FileIds.GetView();

            ModelCombobox.DataContext = Models;
            ModelCombobox.ItemsSource = Models.GetView();

            SetGridFilter();
        }

        /// <summary>
        /// Sets the filter on the SubGrid
        /// </summary>
        private void SetSubGridFilter()
        {
            if (Labels.Selected != null)
                ((ICollectionView)SubGrid.ItemsSource).Filter = item =>
                       !Equals(((Label)item).Language, Languages.Selected)
                       && String.Equals(((Label)item).Id, Labels.Selected.Id,
                           StringComparison.CurrentCultureIgnoreCase)
                       && Equals(((Label)item).FileId, FileIds.Selected)
                    ;
            else
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => false;
        }

        /// <summary>
        /// Sets the filter on the MainGrid
        /// </summary>
        private void SetGridFilter()
        {
            if (MainGrid.ItemsSource == null || SubGrid.ItemsSource == null || Languages.Selected == null)
                return;

            ((ICollectionView)MainGrid.ItemsSource).Filter = item =>
                   Equals(((Label)item).Language, Languages.Selected)
                   && Equals(((Label)item).FileId, FileIds.Selected)
                   && !((Label)item).Deleted
                   && (string.IsNullOrWhiteSpace(SearchString)
                       || ((Label)item).Text.ToLowerInvariant().Contains(SearchString.ToLowerInvariant())
                       || ((Label)item).Id.ToLowerInvariant().Contains(SearchString.ToLowerInvariant()))
                ;
            SetSubGridFilter();
        }

        /// <summary>
        /// Rereads all Labels
        /// </summary>
        public void ReloadLabels(bool reloadModels = false)
        {
            if (!ValidateReload())
                return;

            if (reloadModels)
                Models.Init();

            XmlFiles.Init();
            Languages.Init();
            FileIds.Init();
            ReadFilesNew.Init();

            Timer?.Reset();

            SetGridsReadOnly(false);

            ValidateLockedModel();
        }

        /// <summary>
        /// Sets a new AxLabelPath
        /// </summary>
        /// <returns>True if selection was successful</returns>
        private bool SetPath()
        {
            bool ok;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = AxLabelPath;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AxLabelPath = dialog.SelectedPath;
                    ok = true;
                }
                else
                {
                    ok = false;
                }
            }

            return ok;
        }

        /// <summary>
        /// Moves the focus to the currently selected item
        /// </summary>
        public void MoveToSelectedItem()
        {
            MainGrid.UpdateLayout();
            MainGrid.ScrollIntoView(MainGrid.SelectedItem);
            MainGrid.Focus();
        }

        #region Events

        /// <summary>
        /// Handles a change in the Language control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGridFilter();
        }

        /// <summary>
        /// Handles a change in the FileId control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGridFilter();
        }

        /// <summary>
        /// Tracks if model change was canceled to prevent a infinite loop
        /// </summary>
        private bool _modelChangeWasCanceled;

        /// <summary>
        /// Handles a change in the FileId control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_modelChangeWasCanceled)
            {
                _modelChangeWasCanceled = false;
                e.Handled = true;
                return;
            }

            var response = ShouldSaveAndClose(true);

            if (response.Save)
                Labels.Save();

            if (!response.Close && e.RemovedItems[0] is Model)
                Models.Selected = (Model)e.RemovedItems[0];
            else
                ReloadLabels();
        }

        /// <summary>
        /// Handles the window closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();

            if (!CanClose(e))
            {
                e.Cancel = true;
                return;
            }

            if (!Changed)
                return;

            var response = ShouldSaveAndClose();

            e.Cancel = !response.Close;

            if (response.Save)
                Labels.Save(e);

            try
            {
                Server.Stop();
            }
            catch (ConnectionAlreadyClosedException)
            { }
        }

        /// <summary>
        /// Validation before closing
        /// </summary>
        /// <param name="e"></param>
        /// <returns>True if window can be closed</returns>
        private bool CanClose(CancelEventArgs e)
        {
            var ok = true;


            return ok;
        }

        /// <summary>
        /// Prompts the user if the form should be closed and or saved
        /// </summary>
        /// <returns></returns>
        private CloseSaveResponse ShouldSaveAndClose(bool useModelChangedWasCanceled = false)
        {
            var response = new CloseSaveResponse();

            if (ReadFilesNew.All.Any(x => x.Changed))
            {
                var messageResult = MessageBox.Show(General.FileChangedCantSaveMessage, General.FileChangedCantSaveTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                switch (messageResult)
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        response.Close = false;
                        if (useModelChangedWasCanceled)
                            _modelChangeWasCanceled = true;
                        break;
                }
            }
            else if (Changed)
            {
                var result = MessageBox.Show(General.SaveFileConfirmMessage, General.SaveFileConfirmTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        response.Close = false;
                        if (useModelChangedWasCanceled)
                            _modelChangeWasCanceled = true;
                        break;
                    case System.Windows.Forms.DialogResult.Yes:
                        response.Save = true;
                        break;
                }
            }

            return response;
        }


        /// <summary>
        /// Handles the selecteion changed event of MainGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubGridFilter();
        }

        /// <summary>
        /// Handles the Copy row event
        /// Changes the default behaviour from copying the whole line to only the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grids_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            if (!(sender is DataGrid))
                throw new ArgumentOutOfRangeException();

            var senderGrid = (DataGrid)sender;
            var currentCell = e.ClipboardRowContent[senderGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        /// <summary>
        /// Handles the RenameLabel command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedRoutedEventArgs"></param>
        private void RenameLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (!Labels.ValidateRename())
                return;

            var dialog = new RenameLabel(this, Labels.Selected.Id);

            if (dialog.ShowDialog() ?? false)
                Labels.Rename(dialog.NewIdTextbox.Text);
        }

        /// <summary>
        /// Handles the AddLabel command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedRoutedEventArgs"></param>
        public void AddLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            ShowAddLabelDialog();
        }

        /// <summary>
        /// SHows the <see cref="AddLabel"/> window
        /// </summary>
        /// <param name="labelText">(optional) A preset LabelText</param>
        /// <returns>A list of all added <see cref="Label"/>s</returns>
        public Dictionary<string, List<Label>> ShowAddLabelDialog(string labelText = "")
        {
            if (!ValidateAdd())
            {
                return new Dictionary<string, List<Label>>();
            }

            var dialog = new AddLabel(this, labelText);

            if (dialog.ShowDialog() ?? false)
                return Labels.AddLabel(dialog);

            return new Dictionary<string, List<Label>>();
        }

        /// <summary>
        /// Validating before adding a label
        /// </summary>
        /// <returns>True if Label can be added</returns>
        private bool ValidateAdd()
        {
            var ok = true;

            if (string.IsNullOrWhiteSpace(FileIds.Selected.Name))
            {
                MessageBox.Show(General.Validate_Add_NoFileId_Message, General.Validate_Add_NoFileId_Title);
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Handles the Save command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedRoutedEventArgs"></param>
        private void SaveLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Save();
        }

        /// <summary>
        /// Handles the exit button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles a Delete command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedRoutedEventArgs"></param>
        private void DeleteLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Delete();
        }

        /// <summary>
        /// Handles a Translate command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedRoutedEventArgs"></param>
        private void TranslateLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Translate();
        }

        /// <summary>
        /// Handles a change to a property
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handles the InitColumns event
        /// Sets column properties using attributes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyInfo prop = typeof(Label).GetProperty(e.PropertyName);
            if (prop == null)
                return;

            var attributes = prop.GetCustomAttributes().ToList();
            if (attributes.Count <= 0)
                return;

            var attribute = (MyWpfAttributes)attributes.FirstOrDefault(x => x.GetType() == typeof(MyWpfAttributes));
            if (attribute == null)
                return;

            if (!attribute.Visible)
            {
                e.Cancel = true;
                return;
            }

            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
                e.Column.Header = attribute.DisplayName;

            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }

        /// <summary>
        /// Handles the Reload button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadLabelButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadLabels();
        }

        /// <summary>
        /// Handles the Reload button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadModelButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadLabels(true);
        }

        #endregion

        /// <summary>
        /// Handles the ShowDiff button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDiffButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ShowDiff(this);

            dialog.ShowDialog();
        }

        /// <summary>
        /// Handles a change in the Seach textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetGridFilter();
        }

        /// <summary>
        /// Handles the window loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var updater = new Updater();
            updater.Init();
            updater.Start();
        }

        /// <summary>
        /// Starts the PaseLabel process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PasteLabelToVisualStudio(object sender, RoutedEventArgs e)
        {
            Server.PasteLabel(Labels.Selected.FullId);
        }

        /// <summary>
        /// Handler for the case the file changed in the background
        /// </summary>
        /// <returns>Users choise</returns>
        public bool HandleChangedFile()
        {
            var result = MessageBox.Show(General.FileChangedReloadMessage, General.FileChangedReloadTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            switch (result)
            {
                case System.Windows.Forms.DialogResult.Yes:
                    ReloadLabels(true);
                    return false;
                case System.Windows.Forms.DialogResult.No:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles a clock on the Searchbox clear button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            ResetSearchBox();
        }

        /// <summary>
        /// Empties the searchbox and sets focus
        /// </summary>
        public void ResetSearchBox()
        {
            SearchString = "";
            SearchTextbox.Focus();

        }

        /// <summary>
        /// Validation before reloading
        /// </summary>
        /// <returns></returns>
        private bool ValidateReload()
        {
            bool ok = true;

            return ok;
        }

        /// <summary>
        /// Validates the locked status of the model
        /// </summary>
        private void ValidateLockedModel()
        {
            if (!Models.Selected.Locked)
                return;

            MessageBox.Show(General.Validate_Reload_Locked_Message, General.Validate_Reload_Locked_Title);
            SetGridsReadOnly(true);
        }

        /// <summary>
        /// Sets the readonly property of the grids
        /// </summary>
        /// <param name="readOnly">Should the grids be readOnly</param>
        public void SetGridsReadOnly(bool readOnly)
        {
            MainGrid.IsReadOnly = readOnly;
            SubGrid.IsReadOnly = readOnly;
        }

        private void SetPathButton_Click(object sender, RoutedEventArgs e)
        {
            if (SetPath())
                ReloadLabels(true);
        }

    }
}