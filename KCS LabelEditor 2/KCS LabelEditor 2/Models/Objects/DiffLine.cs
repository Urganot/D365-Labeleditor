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

        [MyWpfAttributes(Visible = Visibility.Hidden)]
        public bool Changed { get; }

        [MyWpfAttributes(Visible = Visibility.Hidden)]
        public bool Added { get; }

        [MyWpfAttributes(Visible = Visibility.Hidden)]
        public bool Deleted { get; }

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