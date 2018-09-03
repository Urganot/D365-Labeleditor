using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AutoComment2015.Helper
{
    public class Helper
    {
        private readonly DTE2 _dte2;

        public Helper(DTE2 dte2)
        {
            _dte2 = dte2;
        }

        private TextSelection _TextSelection => (TextSelection) _dte2.ActiveDocument.Selection;


        /// <summary>
        ///     Gets the TextView for the active document.
        /// </summary>
        /// <returns>
        ///     The current TextView
        /// </returns>
        public static IWpfTextView GetCurrentTextView()
        {
            var componentModel = GetComponentModel();
            if (componentModel == null)
                return null;
            var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            var nativeView = GetCurrentNativeTextView();

            return nativeView == null ? null : editorAdapter.GetWpfTextView(nativeView);
        }

        /// <summary>
        ///     Gets the current native text view
        /// </summary>
        /// <returns>
        ///     The current native text view
        /// </returns>
        private static IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager) ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

            IVsTextView activeView;
            textManager.GetActiveView(1, null, out activeView);
            return activeView;
        }

        /// <summary>
        ///     Gets the current ComponentModel
        /// </summary>
        /// <returns>
        ///     The current ComponentModel
        /// </returns>
        private static IComponentModel GetComponentModel()
        {
            return (IComponentModel) Package.GetGlobalService(typeof(SComponentModel));
        }

        /// <summary>
        ///     Inserts given text
        /// </summary>
        /// <param name="view">The current TextView See <see cref="Helper.GetCurrentTextView()" /></param>
        /// <param name="text">The text</param>
        public void InsertText(IWpfTextView view, string text)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                _dte2.UndoContext.Open("Generate text");

                using (var edit = view.TextBuffer.CreateEdit())
                {
                    if (!view.Selection.IsEmpty)
                    {
                        edit.Delete(view.Selection.SelectedSpans[0].Span);
                        view.Selection.Clear();
                    }

                    edit.Insert(view.Caret.Position.BufferPosition, text);
                    edit.Apply();
                }
            }
            finally
            {
                _dte2.UndoContext.Close();
            }
        }

        public static string CharToLower(string input, int index)
        {
            string text;

            if (index == 0)
            {
                text = input.First().ToString().ToLower() + input.Substring(1);
            }
            else if (index == input.Length - 1)
            {
                text = input.Substring(0, input.Length - 1) + input.Last().ToString().ToLower();
            }
            else
            {
                var sub1 = input.Substring(0, index);
                var sub2 = input.Substring(index + 1, input.Length - (index + 1));

                text = sub1 + input[index].ToString().ToLower() + sub2;
            }

            return text;
        }
    }
}