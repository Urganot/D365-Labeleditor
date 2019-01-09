using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor
{
    /// <summary>
    /// Interaction logic for ShowDiff.xaml
    /// </summary>
    public partial class ShowDiff
    {

        private readonly List<DiffLine> _diffLines = new List<DiffLine>();

        private readonly MainWindow _mainWindow;
        private Language _selectedLanguage;
        private FileId _selectedFileId;

        /// <summary>
        /// The selected language
        /// </summary>
        public Language SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The selected FileId
        /// </summary>
        public FileId SelectedFileId
        {
            get { return _selectedFileId; }
            set
            {
                _selectedFileId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constructor for SHowDiffWindow
        /// </summary>
        /// <param name="mainWindow">An instance of MainWindow</param>
        public ShowDiff(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            foreach (var label in _mainWindow.Labels.All)
            {
                var diffLine = new DiffLine(label);
                _diffLines.Add(diffLine);
            }

            InitializeComponent();
            InitData();
            DataContext = this;
        }

        /// <summary>
        /// Initializes properties
        /// </summary>
        private void InitData()
        {
            DataContext = this;
            DiffGrid.ItemsSource = new CollectionViewSource { Source = _diffLines }.View;

            Languages.ItemsSource = _mainWindow.Languages.GetView();
            FileIds.ItemsSource = _mainWindow.FileIds.GetView();

            SelectedLanguage = _mainWindow.Languages.Selected;
            SelectedFileId = _mainWindow.FileIds.Selected;
        }

        /// <summary>
        /// Handles the InitColumns event.
        /// Sets Column properties via attributes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyInfo prop = typeof(DiffLine).GetProperty(e.PropertyName);
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
        /// Handels the SelectionChanged event of the language control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

        /// <summary>
        /// Handels the SelectionChanged event of the fileId control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileIdCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

        /// <summary>
        /// Sets the filter for the grid
        /// </summary>
        private void SetFilter()
        {
            ((ICollectionView)DiffGrid.ItemsSource).Filter = item => Equals(((DiffLine)item).Language, SelectedLanguage)
                                                                     && Equals(((DiffLine)item).FileId, SelectedFileId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
