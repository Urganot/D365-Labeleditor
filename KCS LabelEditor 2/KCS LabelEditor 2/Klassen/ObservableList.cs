using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KCS_LabelEditor_2
{
    public class ObservableList
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
