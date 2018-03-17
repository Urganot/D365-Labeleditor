using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace KCS_LabelEditor_2
{
    public class Label : INotifyPropertyChanged
    {
        private string _language;
        private string _comment;
        private string _text;
        private string _id;
        private string _fileId;

        private readonly MainWindow _mainWindow;

        public Label(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }


        [MyWpfAttributes(IsReadOnly = true,Width = 8,WidthType = DataGridLengthUnitType.Star)]
        public string FileId
        {
            get => _fileId;
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 17, WidthType = DataGridLengthUnitType.Star)]
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
        public string Language
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