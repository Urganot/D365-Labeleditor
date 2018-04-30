using System;
using System.Collections.Generic;
using System.ServiceModel;
using AVA_LabelEditor.Objects;

namespace AVA_LabelEditor.Server
{
    [ServiceContract(CallbackContract = typeof(ILabelEditorServiceCallBack))]
    public interface ILabelEditorService
    {
        [OperationContract]
        Dictionary<string, List<Label>> CreateNewLabel(string newLabelText);

        [OperationContract]
        void SearchLabel(string searchText);

        [OperationContract]
        void Register(Guid guid);
    }

    [ServiceContract]
    public interface ILabelEditorServiceCallBack
    {
        [OperationContract(IsOneWay = true)]
        void PasteLabel(string message);
    }
}
