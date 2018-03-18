using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace KCS_LabelEditor_2
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

        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }

        }

        public FileId SelectedFileId
        {
            get => _selectedFileId;
            set
            {
                _selectedFileId = value;
                OnPropertyChanged();
            }
        }

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
        }

        private void InitData()
        {
            DataContext = this;
            DiffGrid.ItemsSource = new CollectionViewSource { Source = _diffLines }.View;

            Languages.ItemsSource = _mainWindow.Languages.GetView();
            FileIds.ItemsSource = _mainWindow.FileIds.GetView();

            SelectedLanguage = _mainWindow.Languages.Selected;
            SelectedFileId = _mainWindow.FileIds.Selected;
        }

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

            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
                e.Column.Header = attribute.DisplayName;

            e.Column.Visibility = attribute.Visible;
            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }

        private void LanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

        private void FileIdCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilter();
        }

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
