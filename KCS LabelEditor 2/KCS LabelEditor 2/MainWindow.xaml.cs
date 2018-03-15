using System;
using System.Collections;
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
        public ObservableCollection<FileId> FileIds = new ObservableCollection<FileId>();

        public List<XmlFile> XmlFiles = new List<XmlFile>();
        public List<LabelFile> ReadFiles = new List<LabelFile>();

        public Label SelectedLabel { get; set; }
        public string AxLabelPath
        {
            get => Settings.Default.AxLabelPath;
            set
            {
                Settings.Default.AxLabelPath = value;
                OnPropertyChanged();
            }
        }
        public FileId SelectedFileId
        {
            get => new FileId { Name = Settings.Default.FileId };
            set
            {
                Settings.Default.FileId = value.Name;
                OnPropertyChanged();
            }
        }
        public Language SelectedLanguage
        {
            get => new Language { Name = Settings.Default.Language };
            set
            {
                Settings.Default.Language = value.Name;
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
                GetXmlFiles(AxLabelPath);
            }

            GetLanguages();
            ReadLabelFiles();
        }

        private void SetGridData()
        {
            MainGrid.ItemsSource = GetGridItemList();
            SubGrid.ItemsSource = GetGridItemList();
            Language.ItemsSource = GetLanguagesItemList();
            FileId.ItemsSource = GetFileIdItemList();
            SetFilters();
        }

        public void SetSubGridFilter()
        {
            if (SelectedLabel != null)
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => !String.Equals(((Label)item).Language, SelectedLanguage.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                                                        && String.Equals(((Label)item).Id, SelectedLabel.Id, StringComparison.CurrentCultureIgnoreCase)
                                                                        && String.Equals(((Label)item).FileId, SelectedFileId.ToString(), StringComparison.CurrentCultureIgnoreCase);

            else
                ((ICollectionView)SubGrid.ItemsSource).Filter = item => false;
        }

        public void SetFilters()
        {
            if (MainGrid.ItemsSource == null || SubGrid.ItemsSource == null || SelectedLanguage == null)
                return;

            ((ICollectionView)MainGrid.ItemsSource).Filter = item => String.Equals(((Label)item).Language, SelectedLanguage.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                                                        && String.Equals(((Label)item).FileId, SelectedFileId.ToString(), StringComparison.CurrentCultureIgnoreCase);
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
        private IEnumerable GetFileIdItemList()
        {
            return new CollectionViewSource() { Source = FileIds }.View;
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


        private void GetXmlFiles(string path)
        {
            var files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var rootElement = XDocument.Load(file)?.Root;

                if (rootElement == null)
                    continue;

                var xmlFile = new XmlFile
                {
                    Name = rootElement.Element("Name")?.Value,
                    FileId = new FileId { Name = rootElement.Element("LabelFileId")?.Value },
                    LabelContentFileName = rootElement.Element("LabelContentFileName")?.Value,
                    Language = new Language { Name = rootElement.Element("Language")?.Value ?? "en-Us" }
                };

                XmlFiles.Add(xmlFile);

                AddFileId(xmlFile);
            }
        }

        private void AddFileId(XmlFile xmlFile)
        {
            if (!FileIds.Contains(xmlFile.FileId))
            {
                FileIds.Add(xmlFile.FileId);
            }
        }

        private void GetLanguages()
        {
            foreach (var xmlFile in XmlFiles)
            {
                if (!Languages.Contains(xmlFile.Language))
                    Languages.Add(xmlFile.Language);
            }
        }


        private void ReadLabelFiles()
        {
            foreach (var xmlFile in XmlFiles)
            {
                var filePath = Directory.GetFiles(AxLabelPath, xmlFile.LabelContentFileName, SearchOption.AllDirectories).ToList().Single();
                var lines = File.ReadLines(filePath).ToList();

                for (var i = 0; i < lines.Count(); i++)
                {
                    string line = lines[i];
                    string nextLine = i < lines.Count() - 1 ? lines[i + 1] : "";
                    var comment = "";

                    if (nextLine != null && nextLine.Trim().StartsWith(";"))
                    {
                        comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                        i++;
                    }

                    var splitLine = line.Split('=');

                    var label = new Label
                    {
                        FileId = xmlFile.FileId.ToString(),
                        Language = xmlFile.Language.ToString(),
                        Text = splitLine[1],
                        Id = splitLine[0],
                        Comment = comment
                    };

                    Labels.Add(label);
                }
                ReadFiles.Add(new LabelFile(filePath, xmlFile.Language));
            }

        }

        public void SaveFile()
        {
            foreach (var readFile in ReadFiles)
            {
                using (StreamWriter writer = new StreamWriter(readFile.Path))
                {
                    foreach (var label in Labels.Where(x => x.Language == readFile.Language.ToString()))
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

        private void FileId_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            if (!string.IsNullOrWhiteSpace(AxLabelPath))
            {
                GetXmlFiles(AxLabelPath);
            }

            GetLanguages();
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

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubGridFilter();
        }
        #endregion

    }
}
