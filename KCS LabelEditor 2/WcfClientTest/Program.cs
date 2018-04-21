using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfClientTest.LabelEditorServices;

namespace WcfClientTest
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class LabelEditorServiceCallBack : ILabelEditorServiceCallback
    {
        public void OnNotificationSend(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }

    }

    class Program
    {


        public static LabelEditorServiceClient Proxy
        {
            get
            {
                var ctx = new InstanceContext(new LabelEditorServiceCallBack());
                return new LabelEditorServiceClient(ctx, "NetTcpBinding_ILabelEditorService");
            }
        }
        static void Main(string[] args)
        {

            Proxy.Open();

            Console.ReadKey(); 

        }
    }

}
