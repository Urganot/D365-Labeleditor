using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Lists
{
    public class XmlFiles : ObservableList
    {
        public List<XmlFile> All = new List<XmlFile>();

        public XmlFiles(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public bool ValidateInit()
        {
            bool ok = true;

            ok = Directory.Exists(MainWindow.AxLabelPath);

            return ok;

        }

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

        public void Clear()
        {
            All.Clear();
        }

    }
}
