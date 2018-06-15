using System;
using System.Collections.Generic;
using System.ServiceModel;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LabelEditorService : ILabelEditorService
    {
        private MainWindow MainWindow;

        /// <summary>
        /// Constructor for the LabelEditorService class
        /// </summary>
        /// <param name="mainWindow">An instance of the MainWindow class</param>
        public LabelEditorService(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Handles the request from a client to create a new label
        /// </summary>
        /// <param name="newLabelText">The text to use for a new label</param>
        /// <returns>A dictionary of created labels</returns>
        public Dictionary<string, List<Label>> CreateNewLabel(string newLabelText)
        {
            if (newLabelText == null)
            {
                throw new ArgumentNullException(nameof(newLabelText));
            }

            return MainWindow.ShowAddLabelDialog(newLabelText);
        }

        /// <summary>
        /// Handles the request from a client to search a string
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        public void SearchLabel(string searchText)
        {
            MainWindow.SearchString = searchText;
        }

        /// <summary>
        /// Handles the reguster request from a client
        /// </summary>
        /// <param name="guid"></param>
        public void Register(Guid guid)
        {
            MainWindow.Server.Register(OperationContext.Current.GetCallbackChannel<ILabelEditorServiceCallBack>(),guid);
        }
    }
    
}
