using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace KCS_LabelEditor_2
{
    public class Label : INotifyPropertyChanged
    {
        private Language _language;
        private string _comment;
        private string _text;
        private string _id;
        private FileId _fileId;

        private readonly MainWindow _mainWindow;

        public Label(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }


        [MyWpfAttributes(IsReadOnly = true,Width = 8,WidthType = DataGridLengthUnitType.Star)]
        public FileId FileId
        {
            get => _fileId;
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 25, WidthType = DataGridLengthUnitType.Star)]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 20, WidthType = DataGridLengthUnitType.Star)]
        public string FullId => $@"@{FileId}:{Id}";


        [MyWpfAttributes(IsReadOnly = true, Width = 5, WidthType = DataGridLengthUnitType.Star)]
        public Language Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(Width = 30, WidthType = DataGridLengthUnitType.Star)]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(Width = 20, WidthType = DataGridLengthUnitType.Star)]
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(Visible = Visibility.Hidden,IsReadOnly = true)]
        public string OriginalText { get; set; }


        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            _mainWindow.Changed = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}