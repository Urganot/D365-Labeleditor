using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;
using KCS_LabelEditor_2.Klassen;
using KCS_LabelEditor_2.Properties;
using Label = KCS_LabelEditor_2.Klassen.Label;
using MessageBox = System.Windows.MessageBox;

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

        public ObservableCollection<Label> Labels = new ObservableCollection<Label>();
        public ObservableCollection<Language> Languages = new ObservableCollection<Language>();
        public ObservableCollection<FileId> FileIds = new ObservableCollection<FileId>();

        public List<XmlFile> XmlFiles = new List<XmlFile>();
        public List<LabelFile> ReadFiles = new List<LabelFile>();


        private readonly DispatcherTimer _backGroundChangesTimer = new DispatcherTimer();
        public bool Changed { get; set; }

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
            SetGridData();
            DataContext = this;

            if (Directory.Exists(AxLabelPath))
            {
                GetXmlFiles(AxLabelPath);
            }

            GetLanguages();
            ReadLabelFiles();

            _backGroundChangesTimer.Interval = TimeSpan.FromSeconds(1);
            _backGroundChangesTimer.Tick += BackGroundChangesTimerHandler;

            _backGroundChangesTimer.Start();

        }

        private void BackGroundChangesTimerHandler(object sender, EventArgs e)
        {
            bool changed = false;
            foreach (var labelFile in ReadFiles)
            {
                labelFile.Changed = labelFile.OriginalHash != labelFile.NewHash();
                if (labelFile.Changed)
                {
                    changed = labelFile.Changed;
                    break;
                }
            }

            if (!changed || DontReload)
                return;

            _backGroundChangesTimer.Stop();
            var result = MessageBox.Show("Datei wurde geändert soll diese neu geladen werden?", "Datei geändert", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ReadLabelFiles();
                    break;
                case MessageBoxResult.No:
                    DontReload = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool DontReload { get; set; }


        private void SetGridData()
        {
            MainGrid.ItemsSource = GetGridItemList();
            SubGrid.ItemsSource = GetGridItemList();
            LanguageCombobox.ItemsSource = GetLanguagesItemList();
            FileIdCombobox.ItemsSource = GetFileIdItemList();
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

            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
                e.Column.Header = attribute.DisplayName;
            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }

        private void GetXmlFiles(string path)
        {
            var files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var rootElement = XDocument.Load(file).Root;

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
            Labels.Clear();
            ReadFiles.Clear();
            if (_backGroundChangesTimer != null && !_backGroundChangesTimer.IsEnabled)
                _backGroundChangesTimer.Start();

            foreach (var xmlFile in XmlFiles)
            {
                var filePath = Directory.GetFiles(AxLabelPath, xmlFile.LabelContentFileName, SearchOption.AllDirectories).ToList().Single();
                var lines = File.ReadLines(filePath).ToList();

                for (var i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string nextLine = i < lines.Count - 1 ? lines[i + 1] : "";
                    var comment = "";

                    if (nextLine != null && nextLine.Trim().StartsWith(";"))
                    {
                        comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                        i++;
                    }

                    var splitLine = line.Split('=');

                    var label = new Label(this)
                    {
                        FileId = xmlFile.FileId.ToString(),
                        Language = xmlFile.Language.ToString(),
                        Text = splitLine[1],
                        Id = splitLine[0],
                        Comment = comment
                    };

                    Labels.Add(label);
                }
                ReadFiles.Add(new LabelFile(filePath, xmlFile.Language, xmlFile.FileId));
            }

            Changed = false;

        }

        public void SaveFile()
        {
            _backGroundChangesTimer.Stop();
            foreach (var readFile in ReadFiles)
            {
                using (var writer = new StreamWriter(readFile.Path))
                {
                    foreach (var label in Labels.Where(x => x.Language == readFile.Language.ToString() && x.FileId == readFile.FileId.ToString()))
                    {
                        writer.WriteLine(label.Id + "=" + label.Text);
                        if (!string.IsNullOrWhiteSpace(label.Comment))
                            writer.WriteLine(" ;" + label.Comment);
                    }
                }

                readFile.ResetHash();
            }

            Changed = false;
            _backGroundChangesTimer.Start();
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

            if (ReadFiles.Any(x => x.Changed))
            {
                MessageBoxResult messageResult = MessageBox.Show("Datei wurde geändert. Es kann nicht gespeichert werden!", "Datei wurde geändert",
                     MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                switch (messageResult)
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
                return;
            }

            var result = MessageBox.Show("Labels wurden geändert. Soll gespeichert werden?", "Soll gespeichert werden?",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveFile();
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;

            }
        }

        private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubGridFilter();
        }
        #endregion

        private void AddLabel(string id, string text, string helpText)
        {
            foreach (var language in Languages)
            {
                AddLabel(id, text, language);

                if (!string.IsNullOrWhiteSpace(helpText))
                    AddLabel(id + "Help", helpText, language);
            }
        }

        private void AddLabel(string id, string text, Language language)
        {
            if (AutoTranslate && !Equals(language, SelectedLanguage))
                text = GoogleTranslation.Translation.Translate(text, SelectedLanguage.ToString(), language.ToString());

            var label = new Label(this)
            {
                FileId = SelectedFileId.ToString(),
                Id = id,
                Language = language.ToString(),
                Text = text
            };

            Labels.Add(label);
        }

        public bool IdExists(string id)
        {
            return Labels.Any(x => x.Id == id);
        }

        private void Grids_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var currentCell = e.ClipboardRowContent[MainGrid.CurrentCell.Column.DisplayIndex];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);

        }

        private void RenameLabelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RenameLabel(this, SelectedLabel.Id);

            bool? asd = dialog.ShowDialog();
            if (asd != null && asd == true)
            {
                var newId = dialog.NewIdTextbox.Text;
                Labels.ToList().Where(x => x.FileId.Equals(SelectedLabel.FileId) && x.Id == SelectedLabel.Id).ToList().ForEach(y => y.Id = newId);
            }
        }

        private void AddLabelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddLabel(this);

            bool? asd = dialog.ShowDialog();
            if (asd != null && asd == true)
                AddLabel(dialog.Id.Text, dialog.Text.Text, dialog.HelpText.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReadFiles.Any(x => x.Changed))
            {
                MessageBox.Show("Datei wurde geändert. Es kann nicht gespeichert werden!", "Datei wurde geändert",
                      MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SaveFile();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteLabelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show($"Soll das ausgewählte Label {SelectedLabel.Id} gelöscht werden?", "Label löschen", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                var labelsToRemove = Labels.ToList()
                    .Where(x => x.FileId == SelectedFileId.ToString() && x.Id == SelectedLabel.Id)
                    .ToList();
                foreach (var labelToRemove in labelsToRemove)
                {
                    Labels.Remove(labelToRemove);
                }
            }
        }
    }
}
