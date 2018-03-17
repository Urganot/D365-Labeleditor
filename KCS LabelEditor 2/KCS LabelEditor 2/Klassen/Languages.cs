using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    public class Languages : ObservableList
    {
        public ObservableCollection<Language> All = new ObservableCollection<Language>();

        private readonly MainWindow _mainWindow;

        public Languages(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public Language Selected
        {
            get => new Language(Settings.Default.Language);
            set
            {
                Settings.Default.Language = value?.Name ?? "";
                OnPropertyChanged();
            }
        }
        public Language SelectedOld { get; set; }

        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        public void Init()
        {
            SelectedOld = Selected;
            Clear();

            foreach (var xmlFile in _mainWindow.XmlFiles.All)
            {
                if (!All.Contains(xmlFile.Language))
                    All.Add(xmlFile.Language);
            }

            CheckSelected();
        }

        private void CheckSelected()
        {
            if (All.Contains(SelectedOld))
                Selected = new Language(SelectedOld.ToString());
        }

        public void Clear()
        {
            All.ToList().Clear();
        }


    }
}
