using System.Collections.Generic;
using System.IO;
using System.Linq;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Lists
{
    public class LabelFiles : ObservableList
    {
        /// <summary>
        /// A list of all LabelFiles
        /// </summary>
        public List<LabelFile> All = new List<LabelFile>();

        /// <summary>
        /// Constructor for the LabelFiles class
        /// </summary>
        /// <param name="mainWindow"></param>
        public LabelFiles(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Initializes data
        /// </summary>
        public void Init()
        {
            MainWindow.Labels.Clear();
            All.Clear();

            if (MainWindow.Timer != null && !MainWindow.Timer.IsRunning())
                MainWindow.Timer.Start();

            foreach (var xmlFile in MainWindow.XmlFiles.All)
            {
                var filePath = Directory.GetFiles(MainWindow.Models.Selected.LabelPath, xmlFile.LabelContentFileName, SearchOption.AllDirectories).ToList().Single();
                var lines = File.ReadLines(filePath).ToList();
                var commentSymbol = "";
                for (var i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string nextLine = i < lines.Count - 1 ? lines[i + 1] : string.Empty;
                    var comment = "";

                    if (nextLine != null && nextLine.Trim().StartsWith(";"))
                    {
                        commentSymbol = ";";
                        comment = nextLine.Substring(nextLine.IndexOf(';') + 1);
                        i++;
                    }
                    else if (nextLine != null && nextLine.Trim().StartsWith("#"))
                    {
                        commentSymbol = "#";
                        comment = nextLine.Substring(nextLine.IndexOf('#') + 1);
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
                All.Add(new LabelFile(filePath, xmlFile.Language, xmlFile.FileId, commentSymbol));
            }

            MainWindow.Changed = false;
        }
    }
}
