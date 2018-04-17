using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "LabelEditorService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class LabelEditorService : ILabelEditorService,IDisposable
    {
        public string CreateNewLabel(string newLabelText)
        {
            if (newLabelText == null)
            {
                throw new ArgumentNullException("composite");
            }

            return newLabelText + "sads";
        }

        public void Dispose()
        {

        }
    }
}
