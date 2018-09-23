
namespace AVA_LabelEditor.Objects
{
    public class XmlFile
    {
        /// <summary>
        /// Name of the File
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// FullName of the FIle
        /// </summary>
        public string LabelContentFileName { get; set; }

        /// <summary>
        /// Id of the File
        /// </summary>
        public FileId FileId { get; set; }

        /// <summary>
        /// Language of the file
        /// </summary>
        public Language Language { get; set; }

    }
}