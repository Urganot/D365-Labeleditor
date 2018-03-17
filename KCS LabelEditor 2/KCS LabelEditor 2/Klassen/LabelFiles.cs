using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KCS_LabelEditor_2
{
    public class LabelFiles
    {
        public List<LabelFile> All = new List<LabelFile>();

        private readonly MainWindow _mainWindow;

        public LabelFiles(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Init()
        {
            _mainWindow.Labels.Clear();
            All.Clear();

            if (_mainWindow.Timer != null && !_mainWindow.Timer.IsRunning())
                _mainWindow.Timer.Start();

            foreach (var xmlFile in _mainWindow.XmlFiles.All)
            {
                var filePath = Directory.GetFiles(_mainWindow.AxLabelPath, xmlFile.LabelContentFileName, SearchOption.AllDirectories).ToList().Single();
                var lines = File.ReadLines(filePath).ToList();

                for (var i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string nextLine = i < lines.Count - 1 ? lines[i + 1] : "";
                    var comment = "";

                    if (nextLine != null && nextLine.Trim().StartsWith(";"))
                    {
                        comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                        i++;
                    }

                    var splitLine = line.Split('=');

                    var label = new Label(_mainWindow)
                    {
                        FileId = xmlFile.FileId,
                        Language = xmlFile.Language,
                        Text = splitLine[1],
                        Id = splitLine[0],
                        Comment = comment
                    };

                   _mainWindow.Labels.Add(label);
                }
                All.Add(new LabelFile(filePath, xmlFile.Language, xmlFile.FileId));
            }

            _mainWindow.Changed = false;
        }
    }
}
