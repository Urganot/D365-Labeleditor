using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for halloWelt.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public XmlFiles XmlFiles;
        public LabelFiles ReadFilesNew;
        public Labels Labels;
        public Languages Languages;
        public FileIds FileIds;

        public string SearchString
        {
            get => SearchTextbox.Text;
            set => SearchTextbox.Text = value;
        }

        public BackgroundTimer Timer;

        public bool Changed { get; set; }

        public string AxLabelPath
        {
            get => Settings.Default.AxLabelPath;
            set
            {
                Settings.Default.AxLabelPath = value;
                OnPropertyChanged();
            }
        }

        public bool AutoTranslate
        {
            get => Settings.Default.AutoTranslate;
            set
            {
                Settings.Default.AutoTranslate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Init();

            SetDataBindings();

            if (string.IsNullOrWhiteSpace(AxLabelPath))
                SetPath();

            ReloadLabels();

            InitTimer();
        }

        private void InitTimer()
        {
            Timer = new BackgroundTimer(this);
            Timer.Init();
            Timer.Start();
        }

        private void Init()
        {
            Labels = new Labels(this);
            Languages = new Languages(this);
            FileIds = new FileIds(this);
            XmlFiles = new XmlFiles(this);
            ReadFilesNew = new LabelFiles(this);
        }

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

           // SearchTextbox.DataContext = this;

            SetGridFilter();
        }

        private void SetSubGridFilter()
        {
            if (Labels.Selected != null)
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => !Equals(((Label)item).Language, Languages.Selected)
                                                                        && String.Equals(((Label)item).Id, Labels.Selected.Id, StringComparison.CurrentCultureIgnoreCase)
                                                                        && Equals(((Label)item).FileId, FileIds.Selected)
                                                                        ;
            else
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => false;
        }

        private void SetGridFilter()
        {
            if (MainGrid.ItemsSource == null || SubGrid.ItemsSource == null || Languages.Selected == null)
                return;

            ((ICollectionView)MainGrid.ItemsSource).Filter = item => Equals(((Label)item).Language, Languages.Selected)
                                                                        && Equals(((Label)item).FileId, FileIds.Selected)
                                                                        && !((Label)item).Deleted
                                                                        && (string.IsNullOrWhiteSpace(SearchString) 
                                                                            || ((Label)item).Text.ToLowerInvariant().Contains(SearchString.ToLowerInvariant()) 
                                                                            || ((Label)item).Id.ToLowerInvariant().Contains(SearchString.ToLowerInvariant()))
                                                                        ;
            SetSubGridFilter();
        }

        public void ReloadLabels()
        {
            XmlFiles.Init();
            Languages.Init();
            FileIds.Init();
            ReadFilesNew.Init();
        }

        private void SetPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AxLabelPath = dialog.SelectedPath;
                }
            }
        }

        public void MoveToSelectedItem()
        {
            MainGrid.UpdateLayout();
            MainGrid.ScrollIntoView(MainGrid.SelectedItem);
            MainGrid.Focus();
        }

        #region Events
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGridFilter();
        }

        private void FileId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetGridFilter();
        }

        private void GetPath_Click(object sender, RoutedEventArgs e)
        {
            SetPath();
            ReloadLabels();
        }

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

            Labels.Save(e);

        }

        private bool CanClose(CancelEventArgs e)
        {
            var ok = true;


            return ok;
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubGridFilter();
        }

        private void Grids_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[MainGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }

        private void RenameLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        { 
            if (!Labels.ValidateRename())
                return;

            var dialog = new RenameLabel(this, Labels.Selected.Id);

            if (dialog.ShowDialog() ?? false)
                Labels.Rename(dialog.NewIdTextbox.Text);

        }

        public void AddLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            var dialog = new AddLabel(this);

            if (dialog.ShowDialog() ?? false)
                Labels.AddLabel(dialog);

        }

        private void SaveLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Save();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Delete();
        }

        private void TranslateLabel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Labels.Translate();
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
                e.Column.Header = attribute.DisplayName;

            e.Column.Visibility = attribute.Visible;
            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadLabels();
        }
        #endregion

        private void ShowDiffButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ShowDiff(this);

            dialog.ShowDialog();
        }

        private void SearchTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetGridFilter();

        }

        protected void BlockTheCommand(object sender,
            CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            e.Handled = true;
        }

    }
}
