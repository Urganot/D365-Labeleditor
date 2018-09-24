using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;

namespace AVA_LabelEditor.Lists
{
    public class Models : ObservableList
    {
        public ObservableCollection<Model> All = new ObservableCollection<Model>();

        /// <summary>
        /// Constructor for the <see cref="FileIds"/> class
        /// </summary>
        /// <param name="mainWindow">An instance of the <see cref="MainWindow"/> class</param>
        public Models(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }


        public void Init()
        {
            Clear();

            var directories = Directory.GetDirectories(MainWindow.AxLabelPath, "AxLabelFile", SearchOption.AllDirectories).ToList();

            foreach (var directory in directories)
            {
                var dir = new DirectoryInfo(directory);
                var modelDir = dir.Parent?.Parent;
                if (modelDir == null)
                    continue;
                var descriptor = Directory.GetDirectories(modelDir.FullName, "Descriptor").SingleOrDefault();
                if (string.IsNullOrWhiteSpace(descriptor))
                    continue;
                var file = Directory.GetFiles(descriptor, modelDir.Name + ".xml").SingleOrDefault();
                if (file == null || !File.Exists(file))
                    continue;
                var rootElement = XDocument.Load(file).Root;

                var model = new Model
                {
                    LabelPath = directory,
                    Locked = rootElement?.Element("Locked")?.Value.ToLower() == "true",
                    Name = modelDir.Name,
                    Path = modelDir.FullName
                };

                All.Add(model);
            }

            if (!All.Any())
            {
                MessageBox.Show(Properties.MainWindow.NoModelsFoundMessage, Properties.MainWindow.NoModelsFoundTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// The selected <see cref="Model"/>
        /// </summary>
        public Model Selected
        {
            get { return Settings.Default.Model ?? new Model(); }
            set
            {
                Settings.Default.Model = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Returns the view object to use in the UI
        /// </summary>
        /// <returns>The <see cref="Model"/> <see cref="ICollectionView"/> object</returns>
        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        /// <summary>
        /// Clears the model list
        /// </summary>
        public void Clear()
        {
            All.Clear();
        }
    }
}
