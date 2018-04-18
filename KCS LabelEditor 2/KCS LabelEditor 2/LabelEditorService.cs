using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KCS_LabelEditor_2;

namespace Communication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LabelEditorService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LabelEditorService : ILabelEditorService
    {
        private Labels Labels;
        private MainWindow MainWindow;


        public LabelEditorService(Labels labels, MainWindow mainWindow)
        {
            Labels = labels;
            MainWindow = mainWindow;
        }

        public Dictionary<string, List<Label>> CreateNewLabel(string newLabelText)
        {
            if (newLabelText == null)
            {
                throw new ArgumentNullException("composite");
            }

            return MainWindow.ShowAddLabelDialog(newLabelText);
        }

    }
}
