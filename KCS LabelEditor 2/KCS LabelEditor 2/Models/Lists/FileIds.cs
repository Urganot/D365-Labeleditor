using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;

namespace AVA_LabelEditor.Lists
{
    public class FileIds : ObservableList
    {

        private readonly ObservableCollection<FileId> _all = new ObservableCollection<FileId>();

        public FileIds(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public FileId Selected
        {
            get { return new FileId {Name = Settings.Default.FileId}; }
            set
            {
                Settings.Default.FileId = value?.Name ?? string.Empty;
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

            foreach (var xmlFile in MainWindow.XmlFiles.All)
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
