﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    public interface ITranslator
    {
        TranslationResponse Translate(string sourceText, string sourceLanguage, string targetLanguage);
    }
}
