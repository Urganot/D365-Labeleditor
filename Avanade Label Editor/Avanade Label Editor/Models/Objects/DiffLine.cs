using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AVA_LabelEditor.Helper;
using DiffMatchPatch;
using Label = AVA_LabelEditor.Objects.Label;

namespace AVA_LabelEditor.Objects
{
    public class DiffLine : ObservableList
    {
        private string _id;
        private string _original;
        private string _new;
        private Language _language;
        private FileId _fileId;
        private string _result;

        /// <summary>
        /// The Labels <see cref="FileId"/>
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public FileId FileId
        {
            get { return _fileId; }
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Labels <see cref="Language"/>
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public Language Language
        {
            get
            { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Labels id
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The old text with changes
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string Original
        {
            get { return _original; }
            set
            {
                _original = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// The new text with changes
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string New
        {
            get { return _new; }
            set
            {
                _new = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The new text
        /// </summary>
        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Was the Label changed
        /// </summary>
        [MyWpfAttributes(Visible = false)]
        public bool Changed { get; }

        /// <summary>
        /// Was the Label added
        /// </summary>
        [MyWpfAttributes(Visible = false)]
        public bool Added { get; }

        /// <summary>
        /// Was the Label deleted
        /// </summary>
        [MyWpfAttributes(Visible = false)]
        public bool Deleted { get; }

        /// <summary>
        /// Constructor for the DiffLine class
        /// </summary>
        /// <param name="label">An instance of a Label class </param>
        public DiffLine(Label label)
        {

            Id = label.Id;
            Result = label.Text;
            FileId = label.FileId;
            Language = label.Language;
            Added = string.IsNullOrWhiteSpace(label.OriginalText);
            Deleted = label.Deleted;
            Changed = !Added && label.OriginalText != label.Text;


            if (Added)
            {
                New = label.Text;
            }
            else if (Changed)
            {
                StringBuilder oldText;
                StringBuilder newText;
                CompareText(label, out oldText, out newText);

                Original = oldText.ToString();
                New = newText.ToString();
            }
            else if (Deleted)
            {
                Original = label.Text;
                Result = "";
            }
        }

        /// <summary>
        /// Compares the original und changed text of a label
        /// </summary>
        /// <param name="label">An instance of the Label class to compare</param>
        /// <param name="oldText">A StringBuilder for the displayed changes</param>
        /// <param name="newText">A StringBuilder for the displayed changes</param>
        private static void CompareText(Label label, out StringBuilder oldText, out StringBuilder newText)
        {
            var comp = new diff_match_patch().diff_main(label.OriginalText, label.Text);

            oldText = new StringBuilder();
            newText = new StringBuilder();

            foreach (var diff in comp)
            {
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        oldText.Append("[" + diff.text + "]");
                        newText.Append("[]");
                        break;
                    case Operation.INSERT:
                        oldText.Append("[]");
                        newText.Append("[" + diff.text + "]");
                        break;
                    case Operation.EQUAL:
                        oldText.Append(diff.text);
                        newText.Append(diff.text);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}