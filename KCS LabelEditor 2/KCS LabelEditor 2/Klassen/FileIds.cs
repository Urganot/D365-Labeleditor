using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    public class FileIds : ObservableList
    {

        private readonly ObservableCollection<FileId> _all = new ObservableCollection<FileId>();

        private readonly MainWindow _mainWindow;

        public FileIds(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public FileId Selected
        {
            get => new FileId { Name = Settings.Default.FileId };
            set
            {
                Settings.Default.FileId = value?.Name ?? "";
                OnPropertyChanged();
            }
        }

        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = _all }.View;
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
            if (!_all.Contains(Selected))
                Selected = null;
        }

        public void Add(FileId fileId)
        {
            if (!_all.Contains(fileId))
            {
                _all.Add(fileId);
            }

        }

        public void Clear()
        {
            _all.ToList().Clear();
        }
    }
}
