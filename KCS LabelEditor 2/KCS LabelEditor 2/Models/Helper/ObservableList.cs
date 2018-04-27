using System.ComponentModel;
using System.Runtime.Serialization;

namespace KCS_LabelEditor_2.Helper
{
    [DataContract]
    public class ObservableList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected MainWindow MainWindow;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
