using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
            DataContext = this;


            Labels = new Labels(this);
            Languages = new Languages(this);
            FileIds = new FileIds(this);
            XmlFiles = new XmlFiles(this);
            ReadFilesNew = new LabelFiles(this);

            SetDataBindings();

            if (string.IsNullOrWhiteSpace(AxLabelPath))
                SetPath();

            ReloadLabels();

            Timer = new BackgroundTimer(this);
            Timer.Init();
            Timer.Start();
        }

        private void SetDataBindings()
        {

            MainGrid.DataContext = Labels;
            MainGrid.ItemsSource = Labels.GetView();

            SubGrid.DataContext = Labels;
            SubGrid.ItemsSource = Labels.GetView();

            LanguageCombobox.DataContext = Languages;
            LanguageCombobox.ItemsSource = Languages.GetView();

            FileIdCombobox.DataContext = FileIds;
            FileIdCombobox.ItemsSource = FileIds.GetView();
            SetGridFilter();
        }

        private void SetSubGridFilter()
        {
            if (Labels.Selected != null)
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => !Equals(((Label)item).Language, Languages.Selected)
                                                                        && String.Equals(((Label)item).Id, Labels.Selected.Id, StringComparison.CurrentCultureIgnoreCase)
                                                                        && Equals(((Label)item).FileId, FileIds.Selected);
            else
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => false;
        }

        private void SetGridFilter()
        {
            if (MainGrid.ItemsSource == null || SubGrid.ItemsSource == null || Languages.Selected == null)
                return;

            ((ICollectionView)MainGrid.ItemsSource).Filter = item => Equals(((Label)item).Language, Languages.Selected)
                                                                       && Equals(((Label)item).FileId, FileIds.Selected);
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

            CanClose(e);
        }

        private void CanClose(CancelEventArgs e)
        {
            Labels.Save(e);
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

        private void RenameLabelButton_Click(object sender, RoutedEventArgs e)
        {

            if (!Labels.ValidateRename())
                return;

            var dialog = new RenameLabel(this, Labels.Selected.Id);

            if (dialog.ShowDialog() ?? false)
                Labels.Rename(dialog.NewIdTextbox.Text);

        }

        private void AddLabelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddLabel(this);

            if (dialog.ShowDialog() ?? false)
                Labels.AddLabel(dialog);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Labels.Save();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteLabelButton_Click(object sender, RoutedEventArgs e)
        {
            Labels.Delete();
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
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
    }
}
