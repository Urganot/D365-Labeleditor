using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using KCS_LabelEditor_2.Properties;
using Console = System.Console;

namespace KCS_LabelEditor_2
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Label> Labels = new ObservableCollection<Label>();
        public ObservableCollection<Language> Languages = new ObservableCollection<Language>();

        public Dictionary<Language, string> ReadFiles = new Dictionary<Language, string>();

        public Label SelectedLabel => (Label)MainGrid?.SelectedItem;

        public string AxLabelPath
        {
            get => Settings.Default.AxLabelPath;
            set
            {
                Settings.Default.AxLabelPath = value;
                OnPropertyChanged();
            }
        }

        public Language DefaultLanguage
        {
            get => new Language { Name = Settings.Default.DefaultLanguage };
            set
            {
                Settings.Default.DefaultLanguage = value.Name;
                OnPropertyChanged();
            }
        }

        #endregion


        public MainWindow()
        {
            InitializeComponent();
            SetGridData();
            DataContext = this;

            if (Directory.Exists(AxLabelPath))
            {
                GetFiles(AxLabelPath);
            }
        }

        private void SetGridData()
        {
            MainGrid.ItemsSource = GetGridItemList();
            SubGrid.ItemsSource = GetGridItemList();
            Language.ItemsSource = GetLanguagesItemList();
            SetFilters();
        }

        public void SetSubGridFilter()
        {
            if (SelectedLabel != null)
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => !String.Equals(((Label)item).Language, DefaultLanguage.Name, StringComparison.CurrentCultureIgnoreCase)
                                                                        && String.Equals(((Label)item).Id, SelectedLabel.Id, StringComparison.CurrentCultureIgnoreCase);
            else
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => false;


        }

        public void SetFilters()
        {
            if (MainGrid.ItemsSource == null || SubGrid.ItemsSource == null || DefaultLanguage == null)
                return;

            ((ICollectionView)MainGrid.ItemsSource).Filter = item => String.Equals(((Label)item).Language, DefaultLanguage.Name, StringComparison.CurrentCultureIgnoreCase);
            SetSubGridFilter();
        }

        private ICollectionView GetGridItemList()
        {
            return new CollectionViewSource() { Source = Labels }.View;
        }

        private ICollectionView GetLanguagesItemList()
        {
            return new CollectionViewSource() { Source = Languages }.View;
        }

        private void InitColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var prop = typeof(Label).GetProperty(e.PropertyName);
            if (prop == null)
                return;

            var attributes = prop.GetCustomAttributes().ToList();
            if (attributes.Count <= 0)
                return;

            var attribute = (MyWpfAttributes)attributes.FirstOrDefault(x => x.GetType() == typeof(MyWpfAttributes));
            if (attribute == null)
                return;

            e.Column.Header = attribute.DisplayName;
            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }


        private void GetFiles(string path)
        {
            var files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var xml = XDocument.Load(file);

                var language = new Language { Name = xml?.Root?.Element("Language")?.Value ?? "en-Us" };

                var labelFileId = xml?.Root?.Element("LabelFileId")?.Value;

                if (!Languages.Contains(language))
                    Languages.Add(language);

                var ContentFileName = xml.Root.Element("LabelContentFileName").Value;

                var asd = Directory.GetFiles(path, ContentFileName, SearchOption.AllDirectories).ToList();
                if (asd.Count != 1)
                    throw new ArgumentOutOfRangeException();

                ReadLabelFile(asd.First(), language.Name, labelFileId);
            }
        }


        private void ReadLabelFile(string path, string language, string labelFileId)
        {
            var lines = File.ReadLines(path).ToList();

            for (var i = 0; i < lines.Count(); i++)
            {
                string line = lines[i];
                string nextLine = i < lines.Count() - 1 ? lines[i + 1] : "";
                string comment = "";

                if (nextLine != null && nextLine.Trim().StartsWith(";"))
                {
                    comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                    i++;
                }

                var asd = line.Split('=');
                var id = asd[0];
                var text = asd[1];

                var label = new Label
                {
                    FileId = labelFileId,
                    Language = language,
                    Text = text,
                    Id = id,
                    Comment = comment
                };

                Labels.Add(label);
            }
            ReadFiles.Add(new Language { Name = language }, path);
        }

        public void SaveFile()
        {
            foreach (var readFile in ReadFiles)
            {
                using (StreamWriter writer = new StreamWriter(readFile.Value))
                {
                    foreach (var label in Labels.Where(x=>x.Language==readFile.Key.Name))
                    {
                        writer.WriteLine(label.Id + "=" + label.Text);
                        if (!string.IsNullOrWhiteSpace(label.Comment))
                            writer.WriteLine(" ;" + label.Comment);
                    }
                }
            }
        }

        #region Events
        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFilters();
        }

        private void GetPath_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AxLabelPath = dialog.SelectedPath;
                }
            }
            if (!string.IsNullOrWhiteSpace(Settings.Default.AxLabelPath))
            {
                GetFiles(AxLabelPath);
            }
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Save();
            SaveFile();
        }

        private void SubGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubGridFilter();
        }
        #endregion
    }
}
