using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KCS_LabelEditor_2
{
    public class LabelFiles : ObservableList
    {
        public List<LabelFile> All = new List<LabelFile>();

        public LabelFiles(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public void Init()
        {
            MainWindow.Labels.Clear();
            All.Clear();

            if (MainWindow.Timer != null && !MainWindow.Timer.IsRunning())
                MainWindow.Timer.Start();

            foreach (var xmlFile in MainWindow.XmlFiles.All)
            {
                var filePath = Directory.GetFiles(MainWindow.AxLabelPath, xmlFile.LabelContentFileName, SearchOption.AllDirectories).ToList().Single();
                var lines = File.ReadLines(filePath).ToList();

                for (var i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string nextLine = i < lines.Count - 1 ? lines[i + 1] : string.Empty;
                    var comment = "";

                    if (nextLine != null && nextLine.Trim().StartsWith(";"))
                    {
                        comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                        i++;
                    }

                    var id = line.Substring(0, line.IndexOf('='));
                    var text = line.Substring(line.IndexOf('=')+1);

                    var label = new Label(MainWindow)
                    {
                        FileId = xmlFile.FileId,
                        Language = xmlFile.Language,
                        Id = id,
                        Text = text,
                        Comment = comment,
                        OriginalText = text
                    };

                   MainWindow.Labels.Add(label);
                }
                All.Add(new LabelFile(filePath, xmlFile.Language, xmlFile.FileId));
            }

            MainWindow.Changed = false;
        }
    }
}
