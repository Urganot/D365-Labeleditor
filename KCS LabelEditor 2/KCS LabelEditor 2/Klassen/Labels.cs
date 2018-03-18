using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using KCS_LabelEditor_2.Properties;

namespace KCS_LabelEditor_2
{
    public class Labels : ObservableList
    {

        public ObservableCollection<Label> All = new ObservableCollection<Label>();

        public Label Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        private readonly MainWindow _mainWindow;
        private Label _selected;

        public Labels(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Delete()
        {

            if (!ValidateDelete())
                return;

            var labelsToRemove = All.ToList()
                .Where(x => Equals(x.FileId, Selected.FileId) && x.Id == Selected.Id)
                .ToList();
            foreach (var labelToRemove in labelsToRemove)
            {
                All.Remove(labelToRemove);
            }
        }

        public void Translate()
        {
            All.ToList()
                .Where(x => x.Id == Selected.Id && !Equals(x.Language, Selected.Language) && Equals(x.FileId, Selected.FileId)).ToList()
                .ForEach(y => y.Text = GoogleTranslation.Translation.Translate(Selected.Text, Selected.Language.ToString(), y.Language.ToString()));
        }

        public void Save(EventArgs e = null)
        {
            if (!ValidateSave(e))
                return;

            _mainWindow.Timer.Stop();

            foreach (var readFile in _mainWindow.ReadFilesNew.All)
            {
                WriteFile(readFile);

                readFile.Reset();
            }

            _mainWindow.Changed = false;
            _mainWindow.Timer.Start();
        }

        private void WriteFile(LabelFile readFile)
        {
            using (var writer = new StreamWriter(readFile.Path))
            {
                foreach (var label in All.Where(x => Equals(x.Language, readFile.Language) && Equals(x.FileId, readFile.FileId)))
                {
                    writer.WriteLine(label.Id + "=" + label.Text);
                    if (!string.IsNullOrWhiteSpace(label.Comment))
                        writer.WriteLine(" ;" + label.Comment);
                }
            }
        }

        public void AddNewLabel(string id, string text, Language language)
        {
            if (Settings.Default.AutoTranslate && !Equals(language, _mainWindow.Languages.Selected))
                text = GoogleTranslation.Translation.Translate(text, _mainWindow.Languages.Selected.ToString(), language.ToString());

            var label = new Label(_mainWindow)
            {
                FileId = _mainWindow.FileIds.Selected,
                Id = id,
                Language = language,
                Text = text
            };
            Add(label);

            if (Equals(_mainWindow.Languages.Selected, label.Language))
            {
                Selected = label;
                _mainWindow.MoveToSelectedItem();
            }
        }

        public void Add(Label label)
        {
            All.Add(label);
        }

        public bool IdExists(string id)
        {
            return All.Any(x => x.Id == id);
        }

        private bool ValidateDelete()
        {
            var ok = true;

            if (Selected == null)
            {
                MessageBox.Show("Kein Label ausgewählt", "Kein Label ausgewählt", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                ok = false;
            }

            ok = ok && MessageBoxResult.Yes == MessageBox.Show($"Soll das ausgewählte Label {Selected.Id} gelöscht werden?", "Label löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            return ok;
        }

        private bool ValidateSave(EventArgs e = null)
        {
            var ok = true;

            if (e is CancelEventArgs ex)
            {
                if (_mainWindow.ReadFilesNew.All.Any(x => x.Changed))
                {
                    var messageResult = MessageBox.Show(
                        "Datei wurde geändert. Es kann nicht gespeichert werden!", "Datei wurde geändert",
                        MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    switch (messageResult)
                    {
                        case MessageBoxResult.Cancel:
                            ex.Cancel = true;
                            break;
                    }

                    ok = false;
                }
                else if (_mainWindow.Changed)
                {
                    var result = MessageBox.Show("Labels wurden geändert. Soll gespeichert werden?",
                        "Soll gespeichert werden?",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    switch (result)
                    {
                        case MessageBoxResult.Cancel:
                            ex.Cancel = true;
                            ok = false;
                            break;
                        case MessageBoxResult.No:
                            ok = false;
                            break;
                    }
                }
            }
            else if (_mainWindow.ReadFilesNew.All.Any(x => x.Changed))
            {
                MessageBox.Show("Datei wurde geändert. Es kann nicht gespeichert werden!", "Datei wurde geändert",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ok = false;
            }

            return ok;
        }

        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        public void AddLabel(AddLabel dialog)
        {
            var id = dialog.Id.Text;
            var text = dialog.Text.Text;
            var helpText = dialog.HelpText.Text;
            var viewText = dialog.ViewText.Text;
            var maintainText = dialog.MaintainText.Text;

            foreach (var language in _mainWindow.Languages.All)
            {
                AddNewLabel(id, text, language);

                if (!string.IsNullOrWhiteSpace(helpText))
                    AddNewLabel(id + "Help", helpText, language);

                if (!string.IsNullOrWhiteSpace(viewText))
                    AddNewLabel(id + "View", viewText, language);

                if (!string.IsNullOrWhiteSpace(maintainText))
                    AddNewLabel(id + "Maintain", maintainText, language);
            }
        }

        public void Rename(string newId)
        {
            if (!ValidateRename())
                return;

            All.ToList()
                .Where(x => x.FileId.Equals(Selected.FileId) && x.Id == Selected.Id).ToList()
                .ForEach(y => y.Id = newId);
        }

        public bool ValidateRename()
        {
            var ok = true;

            if (Selected == null)
            {
                MessageBox.Show("Kein Label ausgewählt", "Kein Label ausgewählt", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                ok = false;
            }


            return ok;
        }

        public void Clear()
        {
            All.Clear();
        }
    }
}
