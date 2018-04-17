using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    [ServiceContract]
    public interface ILabelEditorService
    {
        [OperationContract]
        string CreateNewLabel(string newLabelText);
    }
}
