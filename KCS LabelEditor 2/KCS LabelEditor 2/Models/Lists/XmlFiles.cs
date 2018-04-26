using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using KCS_LabelEditor_2.Helper;
using KCS_LabelEditor_2.Objects;

namespace KCS_LabelEditor_2.Lists
{
    public class XmlFiles : ObservableList
    {
        public List<XmlFile> All = new List<XmlFile>();

        public XmlFiles(MainWindow mainWindow)
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
