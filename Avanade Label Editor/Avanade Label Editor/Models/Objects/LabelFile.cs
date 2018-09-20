using System.IO;

namespace AVA_LabelEditor.Objects
{
    public class LabelFile
    {
        /// <summary>
        /// The LabelFiles name
        /// </summary>
        public string Name => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// The LabelFiles original hash
        /// </summary>
        public string OriginalHash { get; private set; }

        /// <summary>
        /// The LabelFiles path
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The LabelFiles language
        /// </summary>
        public Language Language { get; }

        /// <summary>
        /// The LabelFiles id
        /// </summary>
        public FileId FileId { get; }

        /// <summary>
        /// Was the LabelFile changed
        /// </summary>
        public bool Changed { get; set; }

        /// <summary>
        /// The symbol to indicate a comment
        /// </summary>
        public string CommentSymbol { get; set; }

        /// <summary>
        /// Constructor for the LabelFile class
        /// </summary>
        /// <param name="path">Path to the LAaelFile</param>
        /// <param name="language">´The LabelFiles <see cref="Language"/></param>
        /// <param name="fileId">The LabelFiles <see cref="FileId"/></param>
        /// <param name="commentSymbol">The symbol to indicate a comment</param>
        public LabelFile(string path, Language language, FileId fileId, string commentSymbol)
        {
            Path = path;
            OriginalHash = Helper.Helper.Hash(File.ReadAllText(Path));
            Language = language;
            FileId = fileId;
            CommentSymbol = commentSymbol;
        }

        /// <summary>
        /// Gets the current hash for this file
        /// </summary>
        /// <returns>A string containing a hashcode</returns>
        public string NewHash()
        {
            return Helper.Helper.Hash(File.ReadAllText(Path));
        }

        /// <summary>
        /// Sets the original hash to the current
        /// </summary>
        public void Reset()
        {
            Changed = false;
            OriginalHash = Helper.Helper.Hash(File.ReadAllText(Path));
        }
    }
}