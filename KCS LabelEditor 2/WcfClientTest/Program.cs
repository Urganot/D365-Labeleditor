using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfClientTest.LabelEditorServices;

namespace WcfClientTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter zum testen. 0 zum abbrechen");
            string text = Console.ReadLine();
            while (text != "0")
            {
                var proxy = new LabelEditorServiceClient("BasicHttpBinding_ILabelEditorService");

                Console.WriteLine("Added Label: " + proxy.CreateNewLabel(text));

                text = Console.ReadLine();
            }


            Console.ReadKey();

        }
    }
}
