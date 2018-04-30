using System;
using System.Collections.Generic;
using System.ServiceModel;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LabelEditorService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LabelEditorService : ILabelEditorService
    {
        private AVA_LabelEditor.MainWindow MainWindow;


        public LabelEditorService(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public Dictionary<string, List<Label>> CreateNewLabel(string newLabelText)
        {
            if (newLabelText == null)
            {
                throw new ArgumentNullException(nameof(newLabelText));
            }

            return MainWindow.ShowAddLabelDialog(newLabelText);
        }

        public void SearchLabel(string searchText)
        {
            MainWindow.SearchString = searchText;
        }

        public void Register(Guid guid)
        {
            MainWindow.Server.Register(OperationContext.Current.GetCallbackChannel<ILabelEditorServiceCallBack>(),guid);
        }
    }
    
}
