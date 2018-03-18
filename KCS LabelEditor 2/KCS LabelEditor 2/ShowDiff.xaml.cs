using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DiffMatchPatch;

namespace KCS_LabelEditor_2
{
    /// <summary>
    /// Interaction logic for ShowDiff.xaml
    /// </summary>
    public partial class ShowDiff : Window
    {
        private readonly List<DiffLine> _diffLines = new List<DiffLine>();

        public ShowDiff(Labels labels)
        {
            foreach (var label in labels.All)
            {
                var diffLine = new DiffLine(label);
                _diffLines.Add(diffLine);
            }

            InitializeComponent();
            DataContext = this;
            DiffGrid.ItemsSource = _diffLines;
        }

        private void InitColumns(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyInfo prop = typeof(DiffLine).GetProperty(e.PropertyName);
            if (prop == null)
                return;

            var attributes = prop.GetCustomAttributes().ToList();
            if (attributes.Count <= 0)
                return;

            var attribute = (MyWpfAttributes)attributes.FirstOrDefault(x => x.GetType() == typeof(MyWpfAttributes));
            if (attribute == null)
                return;

            if (!string.IsNullOrWhiteSpace(attribute.DisplayName))
                e.Column.Header = attribute.DisplayName;

            e.Column.Visibility = attribute.Visible;
            e.Column.Width = new DataGridLength(attribute.Width, attribute.WidthType);
            e.Column.IsReadOnly = attribute.IsReadOnly;
        }

    }

    public class DiffLine : ObservableList
    {
        private string _id;
        private string _original;
        private string _new;
        private Language _language;
        private FileId _fileId;
        private bool _changed;
        private string _result;

        public DiffLine(Label label)
        {
            CompareText(label, out var oldText, out var newText);

            Id = label.Id;
            Original = oldText.ToString();
            New = newText.ToString();
            Result = label.Text;
            FileId = label.FileId;
            Language = label.Language;
            Changed = label.OriginalText != label.Text;

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
        public bool Changed
        {
            get => _changed;
            set
            {
                _changed = value;
                OnPropertyChanged();
            }
        }
    }
}
