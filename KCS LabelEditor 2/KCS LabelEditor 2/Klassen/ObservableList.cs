using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KCS_LabelEditor_2
{
    public class ObservableList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected MainWindow MainWindow;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
