using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public class TranslationResponse
    {
        /// <summary>
        /// Was the translation successful
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Contains an error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Contains the translated text
        /// </summary>
        public string TranslatedText { get; set; }
    }
}
