using System.IO;

namespace KCS_LabelEditor_2.Klassen
{
    public class LabelFile
    {

        public string Name => System.IO.Path.GetFileName(Path);
        public string Hash { get; }
        public string Path { get; }
        public Language Language { get; }
        public FileId FileId { get ; }

        public LabelFile(string path, Language language, FileId fileId)
        {
            Path = path;
            Hash = Helper.Hash(File.ReadAllText(Path));
            Language = language;
            FileId = fileId;
        }

    }
}