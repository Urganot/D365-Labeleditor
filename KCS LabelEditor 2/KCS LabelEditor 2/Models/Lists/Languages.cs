using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;

namespace AVA_LabelEditor.Lists
{
    public class Languages : ObservableList
    {
        public ObservableCollection<Language> All = new ObservableCollection<Language>();

        public Languages(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public Language Selected
        {
            get { return new Language(Settings.Default.Language); }
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
            Clear();

            foreach (var xmlFile in MainWindow.XmlFiles.All)
            {
                if (!All.Contains(xmlFile.Language))
                    All.Add(xmlFile.Language);
            }

            CheckSelected();
        }

        private void CheckSelected()
        {
            if (!All.Contains(Selected))
                Selected = null;
        }

        public void Clear()
        {
            All.ToList().Clear();
        }


    }
}
