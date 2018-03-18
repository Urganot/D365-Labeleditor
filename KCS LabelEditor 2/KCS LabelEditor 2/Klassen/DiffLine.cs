using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DiffMatchPatch;

namespace KCS_LabelEditor_2
{
    public class DiffLine : ObservableList
    {
        public enum alteration
        {
            None
            , Edited
            , Delted
            , Added
        }


        private string _id;
        private string _original;
        private string _new;
        private Language _language;
        private FileId _fileId;
        private string _result;


        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public FileId FileId
        {
            get => _fileId;
            set
            {
                _fileId = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public Language Language
        {
            get => _language;
            set
            {
                _language = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 10, WidthType = DataGridLengthUnitType.Star)]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string Original
        {
            get => _original;
            set
            {
                _original = value;
                OnPropertyChanged();
            }
        }

        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string New
        {
            get => _new;
            set
            {
                _new = value;
                OnPropertyChanged();
            }
        }
        [MyWpfAttributes(IsReadOnly = true, Width = 35, WidthType = DataGridLengthUnitType.Star)]
        public string Result
        {
            get => _result;
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
                CompareText(label, out var oldText, out var newText);

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