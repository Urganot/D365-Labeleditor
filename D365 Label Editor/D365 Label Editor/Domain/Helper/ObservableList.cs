using System.ComponentModel;
using System.Runtime.Serialization;

namespace AVA_LabelEditor.Helper
{
    /// <summary>
    /// Baseclass for an observable list
    /// </summary>
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
