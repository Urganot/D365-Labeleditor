using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHttp.Http;

namespace Translator
{
    public class GoogleTranslator : ITranslator
    {
        /// <summary>
        /// Path to googles translation api
        /// </summary>
        private readonly string _url;

        public GoogleTranslator()
        {
            _url = "https://translate.googleapis.com/translate_a/single";
        }

        /// <summary>
        /// Translated a given text to the specified language
        /// </summary>
        /// <param name="sourceText">The text to translate</param>
        /// <param name="sourceLanguage">The language from wich to translate</param>
        /// <param name="targetLanguage">The language to translate to</param>
        /// <returns>Translated text</returns>
        public TranslationResponse Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            HttpResponse htmlResponse = GetResponse(sourceText, sourceLanguage, targetLanguage);
            object[] sentences;
            try
            {
                sentences = GetTranslatedSentences(htmlResponse);
            }
            catch (Exception)
            {
                return new TranslationResponse
                {
                    Success = false,
                    Message = Properties.Resources.TranslationFailed,
                    TranslatedText = sourceText
                };
            }

            return new TranslationResponse
            {
                TranslatedText = GetTranslation(sentences)
            };
        }

        /// <summary>
        /// Concats all translated sentences
        /// </summary>
        /// <param name="sentences">Translated sentences</param>
        /// <returns>Translated text</returns>
        private string GetTranslation(object[] sentences)
        {
            var translation = new StringBuilder();

            if (sentences == null)
            {
                return "";
            }

            foreach (object[] sentence in sentences)
            {
                translation.Append(sentence[0]);
            }

            return translation.ToString();
        }

        /// <summary>
        /// Gets the translated sentences from the html response
        /// </summary>
        /// <param name="htmlResponse">Html response</param>
        /// <returns>Translated sentences</returns>
        private object[] GetTranslatedSentences(HttpResponse htmlResponse)
        {

            var response = htmlResponse.DynamicBody as object[];
            var sentences = response?[0] as object[];
            return sentences;

        }

        /// <summary>
        /// Gets response from google api
        /// </summary>
        /// <param name="sourceText">The text to translate</param>
        /// <param name="sourceLanguage">The language from wich to translate</param>
        /// <param name="targetLanguage">The language to translate to</param>
        /// <returns>HTML response</returns>
        private HttpResponse GetResponse(string sourceText, string sourceLanguage, string targetLanguage)
        {
            var http = GetHttpClient();
            var htmlResponse = http.Get(_url, new { client = "gtx", dt = "t", sl = sourceLanguage, tl = targetLanguage, q = sourceText });
            return htmlResponse;
        }

        /// <summary>
        /// Returns an HttpClient
        /// </summary>
        /// <returns>HttpClient</returns>
        private HttpClient GetHttpClient()
        {
            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;
            return http;
        }


    }


}
