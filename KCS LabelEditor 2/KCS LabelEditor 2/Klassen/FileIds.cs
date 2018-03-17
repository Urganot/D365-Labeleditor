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
    public class FileIds : ObservableList
    {

        public ObservableCollection<FileId> All = new ObservableCollection<FileId>();

        private MainWindow _mainWindow;

        public FileIds(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public FileId Selected
        {
            get => new FileId { Name = Settings.Default.FileId };
            set
            {
                Settings.Default.FileId = value.Name;
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
                Add(xmlFile.FileId);
            }

            CheckSelected();
        }
        private void CheckSelected()
        {
            if (!All.Contains(Selected))
                Selected = null;
        }

        public void Add(FileId fileId)
        {
            if (!All.Contains(fileId))
            {
                All.Add(fileId);
            }

        }

        public void Clear()
        {
            All.Clear();
        }
    }
}
