using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyHttp.Http;

namespace GoogleTranslation
{
    public class Translation
    {
        /// <summary>
        /// Path to googles translation api
        /// </summary>
        private const string Url = "https://translate.googleapis.com/translate_a/single";

        /// <summary>
        /// Translated a given text to the specified language
        /// </summary>
        /// <param name="sourceText">The text to translate</param>
        /// <param name="sourceLanguage">The language from wich to translate</param>
        /// <param name="targetLanguage">The language to translate to</param>
        /// <returns>Translated text</returns>
        public static string Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            HttpResponse htmlResponse = GetResponse(sourceText, sourceLanguage, targetLanguage);
            object[] sentences = GetTranslatedSentences(htmlResponse);

            return GetTranslation(sentences);
        }

        /// <summary>
        /// Concats all translated sentences
        /// </summary>
        /// <param name="sentences">Translated sentences</param>
        /// <returns>Translated text</returns>
        private static string GetTranslation(object[] sentences)
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
        private static object[] GetTranslatedSentences(HttpResponse htmlResponse)
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
        private static HttpResponse GetResponse(string sourceText, string sourceLanguage, string targetLanguage)
        {
            var http = GetHttpClient();
            var htmlResponse = http.Get(Url, new { client = "gtx", dt = "t", sl = sourceLanguage, tl = targetLanguage, q = sourceText });
            return htmlResponse;
        }

        /// <summary>
        /// Returns an HttpClient
        /// </summary>
        /// <returns>HttpClient</returns>
        private static HttpClient GetHttpClient()
        {
            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;
            return http;
        }
    }
}
