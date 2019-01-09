using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AutoComment2015.Helper;
using D365_Label_Editor_Extension.LabelEditorServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace D365_Label_Editor_Extension
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class LabelEditorServiceCallBack : ILabelEditorServiceCallback
    {
        public static IServiceProvider ServiceProvider;

        public void PasteLabel(string message)
        {
            var dte2 = (DTE2)ServiceProvider.GetService(typeof(DTE));

            var helper = new Helper(dte2);

            helper.InsertText(Helper.GetCurrentTextView(), message);
        }
    }

}
