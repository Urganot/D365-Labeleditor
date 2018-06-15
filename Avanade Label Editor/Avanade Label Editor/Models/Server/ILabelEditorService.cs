using System;
using System.Collections.Generic;
using System.ServiceModel;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Server
{
    [ServiceContract(CallbackContract = typeof(ILabelEditorServiceCallBack))]
    public interface ILabelEditorService
    {
        /// <summary>
        /// Handles the request from a client to create a new label
        /// </summary>
        /// <param name="newLabelText">The text to use for a new label</param>
        /// <returns>A dictionary of created labels</returns>
        [OperationContract]
        Dictionary<string, List<Label>> CreateNewLabel(string newLabelText);

        /// <summary>
        /// Handles the request from a client to search a string
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        [OperationContract]
        void SearchLabel(string searchText);

        /// <summary>
        /// Handles the reguster request from a client
        /// </summary>
        /// <param name="guid"></param>
        [OperationContract]
        void Register(Guid guid);
    }

    [ServiceContract]
    public interface ILabelEditorServiceCallBack
    {
        /// <summary>
        /// Callbackmethod to handle PaseLabel
        /// </summary>
        /// <param name="message">Message to pase</param>
        [OperationContract(IsOneWay = true)]
        void PasteLabel(string message);
    }
}
