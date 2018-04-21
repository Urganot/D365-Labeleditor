using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using KCS_LabelEditor_2;

namespace Communication
{
    [ServiceContract]
    public interface ILabelEditorService
    {
        [OperationContract]
        Dictionary<string, List<Label>> CreateNewLabel(string newLabelText);

        [OperationContract]
        void SearchLabel(string searchText);
    }
}
