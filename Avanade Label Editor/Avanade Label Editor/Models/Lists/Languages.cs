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
        /// <summary>
        /// List of all available <see cref="Language"/>s
        /// </summary>
        public ObservableCollection<Language> All = new ObservableCollection<Language>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainWindow"></param>
        public Languages(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// The selected <see cref="Language"/>
        /// </summary>
        public Language Selected
        {
            get { return new Language(Settings.Default.Language); }
            set
            {
                Settings.Default.Language = value?.Name ?? "";
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Returns the view object to use in the UI
        /// </summary>
        /// <returns>The <see cref="Languages"/> <see cref="ICollectionView"/> object</returns>
        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        /// <summary>
        /// Initializes the data
        /// </summary>
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

        /// <summary>
        /// Checks if selected <see cref="Language"/> exists
        /// </summary>
        private void CheckSelected()
        {
            if (!All.Contains(Selected))
                Selected = null;
        }

        /// <summary>
        /// Empties the data
        /// </summary>
        public void Clear()
        {
            All.ToList().Clear();
        }


    }
}
