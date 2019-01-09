using System;
using System.Text;
using EasyHttp.Http;
using System.Windows;

namespace Translator
{
    public class Translator
    {
        public static ITranslator Get()
        {
            return new GoogleTranslator();
        }
    }
}
