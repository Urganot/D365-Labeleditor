using System.ComponentModel;
using System.Runtime.Serialization;

namespace AVA_LabelEditor.Helper
{
    [DataContract]
    public class ObservableList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected AVA_LabelEditor.MainWindow MainWindow;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
