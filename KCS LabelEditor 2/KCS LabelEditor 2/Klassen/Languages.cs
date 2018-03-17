using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2.Klassen
{
    public class Languages : ObservableList
    {
        public ObservableCollection<Language> All = new ObservableCollection<Language>();

        private MainWindow _mainWindow;

        public Languages(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public Language Selected
        {
            get => new Language { Name = Settings.Default.Language };
            set
            {
                Settings.Default.Language = value.Name;
                OnPropertyChanged();
            }
        }

        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        public void Init()
        {
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
            if (!All.Contains(Selected))
                Selected = null;
        }

        public void Clear()
        {
            All.Clear();
        }


    }
}
