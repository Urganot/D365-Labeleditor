using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Lists
{
    public class XmlFiles : ObservableList
    {
        /// <summary>
        /// A list of all XmlFiles
        /// </summary>
        public List<XmlFile> All = new List<XmlFile>();

        /// <summary>
        /// Constructor of the XmlFiles class
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow class</param>
        public XmlFiles(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Validates data before initializing
        /// </summary>
        /// <returns>True if data is valid</returns>
        public bool ValidateInit()
        {
            bool ok = true;

            ok = Directory.Exists(MainWindow.AxLabelPath);

            return ok;

        }

        /// <summary>
        /// Initializes the <see cref="XmlFiles"/> data
        /// </summary>
        public void Init()
        {
            Clear();

            if (!ValidateInit())
                return;

            var files = Directory.GetFiles(MainWindow.AxLabelPath, "*.xml", SearchOption.TopDirectoryOnly);
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
                    Language = new Language(rootElement.Element("Language")?.Value ?? "en-Us")
                };

                All.Add(xmlFile);
            }
        }

        /// <summary>
        /// Clears the <see cref="XmlFiles"/> data
        /// </summary>
        public void Clear()
        {
            All.Clear();
        }

    }
}
