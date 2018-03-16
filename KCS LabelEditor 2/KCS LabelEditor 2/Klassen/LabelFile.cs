using System.IO;

namespace KCS_LabelEditor_2.Klassen
{
    public class LabelFile
    {

        public string Name => System.IO.Path.GetFileName(Path);
        public string OriginalHash { get; private set; }
        public string Path { get; }
        public Language Language { get; }
        public FileId FileId { get ; }
        public bool Changed { get; set; }

        public LabelFile(string path, Language language, FileId fileId)
        {
            Path = path;
            OriginalHash = Helper.Hash(File.ReadAllText(Path));
            Language = language;
            FileId = fileId;
        }

        public string NewHash()
        {
            return Helper.Hash(File.ReadAllText(Path));
        }

        public void ResetHash()
        {
            OriginalHash = Helper.Hash(File.ReadAllText(Path));
        }
    }
}