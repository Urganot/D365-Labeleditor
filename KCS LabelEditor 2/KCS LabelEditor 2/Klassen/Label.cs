using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KCS_LabelEditor_2.Klassen
{
    public class Label : INotifyPropertyChanged
    {
        private string _language;
        private string _comment;
        private string _text;
        private string _id;
        private string _fileId;

        
        [MyWpfAttributes(IsReadOnly = true)]
        public string FileId
        {
            get => _fileId;
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true)]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string FullId => $@"@{FileId}:{Id}";

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true)]
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        private MainWindow _mainWindow;

        public Label(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName=null)
        {
            _mainWindow.Changed = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}