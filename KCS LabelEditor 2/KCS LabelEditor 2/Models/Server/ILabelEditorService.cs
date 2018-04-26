using System.Collections.Generic;
using System.ServiceModel;
using KCS_LabelEditor_2.Objects;

namespace KCS_LabelEditor_2.Server
{
    [ServiceContract(CallbackContract = typeof(ILabelEditorServiceCallBack))]
    public interface ILabelEditorService
    {
        [OperationContract]
        Dictionary<string, List<Label>> CreateNewLabel(string newLabelText);

        [OperationContract]
        void SearchLabel(string searchText);

        [OperationContract]
        void Register();
    }

    [ServiceContract]
    public interface ILabelEditorServiceCallBack
    {
        [OperationContract(IsOneWay = true)]
        void PasteLabel(string message);
    }
}
