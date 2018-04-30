using System.IO;

namespace AVA_LabelEditor.Objects
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
            OriginalHash = Helper.Helper.Hash(File.ReadAllText(Path));
            Language = language;
            FileId = fileId;
        }

        public string NewHash()
        {
            return Helper.Helper.Hash(File.ReadAllText(Path));
        }

        public void Reset()
        {
            Changed = false;
            OriginalHash = Helper.Helper.Hash(File.ReadAllText(Path));
        }
    }
}