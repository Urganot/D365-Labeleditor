using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using AVA_LabelEditor.Helper;

namespace AVA_LabelEditor.Objects
{
    /// <summary>
    /// This class represents a physical Label
    /// </summary>
    [DataContract]
    public class Label : ObservableList
    {
        private Language _language;
        private string _comment = "";
        private string _text = "";
        private string _id = "";
        private FileId _fileId;
        private bool _deleted;

        /// <summary>
        /// Constructor for the Label class
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow class</param>
        public Label(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// <see cref="FileId"/>
        /// </summary>
        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 8, WidthType = DataGridLengthUnitType.Star,Visible = false)]
        public FileId FileId
        {
            get { return _fileId; }
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// The label id
        /// </summary>
        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 25, WidthType = DataGridLengthUnitType.Star)]
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Labels full id
        /// Use this in Visual Studio
        /// </summary>
        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 20, WidthType = DataGridLengthUnitType.Star)]
        public string FullId
        {
            get { return $@"@{FileId}:{Id}"; }
            protected set { }
        }

        /// <summary>
        /// <see cref="Language"/>
        /// </summary>
        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 5, WidthType = DataGridLengthUnitType.Star)]
        public Language Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Text
        /// </summary>
        [DataMember]
        [MyWpfAttributes(Width = 30, WidthType = DataGridLengthUnitType.Star)]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Comment
        /// </summary>
        [DataMember]
        [MyWpfAttributes(Width = 20, WidthType = DataGridLengthUnitType.Star)]
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Original text
        /// </summary>
        [DataMember]
        [MyWpfAttributes(Visible = false, IsReadOnly = true)]
        public string OriginalText { get; set; } = "";

        /// <summary>
        /// Flag for deletion
        /// </summary>
        [DataMember]
        [MyWpfAttributes(Visible = false, IsReadOnly = true)]
        public bool Deleted
        {
            get { return _deleted; }
            set
            {
                _deleted = value;
                OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            MainWindow.Changed = true;
        }


    }

}