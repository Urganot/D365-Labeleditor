using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using AVA_LabelEditor.Helper;

namespace AVA_LabelEditor.Objects
{
    [DataContract]
    public class Label : ObservableList
    {
        private Language _language;
        private string _comment = "";
        private string _text = "";
        private string _id = "";
        private FileId _fileId;

        private bool _deleted;

        public Label(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 8, WidthType = DataGridLengthUnitType.Star)]
        public FileId FileId
        {
            get { return _fileId; }
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

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

        [DataMember]
        [MyWpfAttributes(IsReadOnly = true, Width = 20, WidthType = DataGridLengthUnitType.Star)]
        public string FullId
        {
            get { return $@"@{FileId}:{Id}"; }
            protected set { }
        }


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

        [DataMember]
        [MyWpfAttributes(Visible = Visibility.Hidden, IsReadOnly = true)]
        public string OriginalText { get; set; } = "";

        [DataMember]
        [MyWpfAttributes(Visible = Visibility.Hidden, IsReadOnly = true)]
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