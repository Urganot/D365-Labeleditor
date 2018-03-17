using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KCS_LabelEditor_2.Klassen
{
    public class XmlFiles
    {
        public List<XmlFile> All = new List<XmlFile>();
        private readonly MainWindow _mainWindow;

        public XmlFiles(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public bool ValidateInit()
        {
            bool ok = true;

            ok = Directory.Exists(_mainWindow.AxLabelPath);

            return ok;

        }

        public void Init()
        {
            Clear();

            if (!ValidateInit())
                return;

            var files = Directory.GetFiles(_mainWindow.AxLabelPath, "*.xml", SearchOption.TopDirectoryOnly);
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
                    Language = new Language { Name = rootElement.Element("Language")?.Value ?? "en-Us" }
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
