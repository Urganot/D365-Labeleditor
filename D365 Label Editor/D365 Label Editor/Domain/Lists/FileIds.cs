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
        /// <summary>
        /// The list of all available <see cref="FileId"/>s
        /// </summary>
        public ObservableCollection<FileId> All = new ObservableCollection<FileId>();

        /// <summary>
        /// Constructor for the <see cref="FileIds"/> class
        /// </summary>
        /// <param name="mainWindow">An instance of the <see cref="MainWindow"/> class</param>
        public FileIds(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// The selected <see cref="FileId"/>
        /// </summary>
        public FileId Selected
        {
            get { return new FileId { Name = Settings.Default.FileId }; }
            set
            {
                Settings.Default.FileId = value?.Name ?? string.Empty;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Returns the view object to use in the UI
        /// </summary>
        /// <returns>The <see cref="FileIds"/> <see cref="ICollectionView"/> object</returns>
        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        /// <summary>
        /// Inits the data
        /// </summary>
        public void Init()
        {
            Clear();

            foreach (var xmlFile in MainWindow.XmlFiles.All)
            {
                Add(xmlFile.FileId);
            }

            CheckForSingleEntry();

            CheckSelected();
        }

        /// <summary>
        /// If there is only one entry set this as selected
        /// </summary>
        public void CheckForSingleEntry()
        {
            if (All.Count == 1)
                Selected = All.Single();
        }


        /// <summary>
        /// Checks if the selected <see cref="FileId"/> exists
        /// </summary>
        private void CheckSelected()
        {
            if (!All.Contains(Selected))
                Selected = null;
        }

        /// <summary>
        /// Adds a new <see cref="FileId"/>
        /// </summary>
        /// <param name="fileId">The <see cref="FileId"/> to add</param>
        public void Add(FileId fileId)
        {
            if (!All.Contains(fileId))
            {
                All.Add(fileId);
            }

        }

        /// <summary>
        /// Clears the list of <see cref="FileId"/>s
        /// </summary>
        public void Clear()
        {
            All.Clear();
        }
    }
}
