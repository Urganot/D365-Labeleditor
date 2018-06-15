using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using AVA_LabelEditor.Helper;
using AVA_LabelEditor.Objects;
using AVA_LabelEditor.Properties;
using GoogleTranslate;
using AddLabel = AVA_LabelEditor.AddLabel;

namespace AVA_LabelEditor.Lists
{
    public class Labels : ObservableList
    {

        public ObservableCollection<Label> All = new ObservableCollection<Label>();

        public Label Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        private Label _selected;

        public Labels(AVA_LabelEditor.MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public void Delete()
        {

            if (!ValidateDelete())
                return;

            All.ToList()
                .Where(x => Equals(x.FileId, Selected.FileId) && x.Id == Selected.Id).ToList()
                .ForEach(x => x.Deleted = true);

            ((ICollectionView)MainWindow.MainGrid.ItemsSource).Refresh();
        }

        public void Translate()
        {
            if (!ValidateTranslate())
                return;

            All.ToList()
                .Where(x => x.Id == Selected.Id && !Equals(x.Language, Selected.Language) && Equals(x.FileId, Selected.FileId)).ToList()
                .ForEach(y => y.Text = Translation.Translate(Selected.Text, Selected.Language.ToString(), y.Language.ToString()));
        }

        private bool ValidateTranslate()
        {
            var ok = true;

            if (Selected == null)
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);

            return ok;
        }

        public void Save(EventArgs e = null)
        {
            if (!ValidateSave(e))
                return;

            MainWindow.Timer.Stop();

            foreach (var readFile in MainWindow.ReadFilesNew.All)
            {
                WriteFile(readFile);

                readFile.Reset();
            }

            MainWindow.Changed = false;
            MainWindow.Timer.Start();
        }

        private void WriteFile(LabelFile readFile)
        {
            using (var writer = new StreamWriter(readFile.Path))
            {
                foreach (var label in All.Where(x => Equals(x.Language, readFile.Language) && Equals(x.FileId, readFile.FileId) && !x.Deleted))
                {
                    writer.WriteLine(label.Id + "=" + label.Text);
                    if (!string.IsNullOrWhiteSpace(label.Comment))
                        writer.WriteLine(" ;" + label.Comment);
                }
            }
        }

        public Label AddNewLabel(string id, string text, Language language)
        {
            if (Settings.Default.AutoTranslate && !Equals(language, MainWindow.Languages.Selected))
                text = Translation.Translate(text, MainWindow.Languages.Selected.ToString(), language.ToString());

            var label = new Label(MainWindow)
            {
                FileId = MainWindow.FileIds.Selected,
                Id = id,
                Language = language,
                Text = text
            };
            Add(label);

            if (Equals(MainWindow.Languages.Selected, label.Language))
            {
                Selected = label;
                MainWindow.MoveToSelectedItem();
            }

            return label;
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
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);

            var result = MessageBox.Show(string.Format(General.DeleteLabelConfirmMessage, Selected.Id), General.DeleteLabelConfirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            ok = ok && result == MessageBoxResult.Yes;

            return ok;
        }

        private bool ValidateSave(EventArgs e = null)
        {
            var ok = true;

            if (MainWindow.IsModelLocked())
                ok = Helper.Helper.CheckFailed(General.Validate_Reload_Locked_Message, General.Validate_Reload_Locked_Title);

            if (e is CancelEventArgs)
            {
                var ex = (CancelEventArgs)e;

                if (MainWindow.ReadFilesNew.All.Any(x => x.Changed))
                {
                    var messageResult = MessageBox.Show(General.FileChangedCantSaveMessage, General.FileChangedCantSaveTitle, MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    switch (messageResult)
                    {
                        case MessageBoxResult.Cancel:
                            ex.Cancel = true;
                            break;
                    }

                    ok = false;
                }
                else if (MainWindow.Changed)
                {
                    var result = MessageBox.Show(General.SaveFileConfirmMessage, General.SaveFileConfirmTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

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
            else if (MainWindow.ReadFilesNew.All.Any(x => x.Changed))
            {
                ok = Helper.Helper.CheckFailed(General.FileChangedCantSaveMessage, General.FileChangedCantSaveTitle);
            }

            return ok;
        }

        public ICollectionView GetView()
        {
            return new CollectionViewSource { Source = All }.View;
        }

        /// <summary>
        /// Adds Label to List
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns>Id of inserted Label</returns>
        public Dictionary<string, List<Label>> AddLabel(AddLabel dialog)
        {
            var id = dialog.Id.Text;
            var text = dialog.Text.Text;
            var helpText = dialog.HelpText.Text;
            var viewText = dialog.ViewText.Text;
            var maintainText = dialog.MaintainText.Text;

            var addedLabels = new Dictionary<string, List<Label>>();

            foreach (var language in MainWindow.Languages.All)
            {
                if (addedLabels.ContainsKey("base"))
                    addedLabels["base"].Add(AddNewLabel(id, text, language));
                else
                    addedLabels.Add("base", new List<Label> { AddNewLabel(id, text, language) });

                if (!string.IsNullOrWhiteSpace(helpText))
                {
                    if (addedLabels.ContainsKey("help"))
                        addedLabels["help"].Add(AddNewLabel(id + "Help", helpText, language));
                    else
                        addedLabels.Add("help", new List<Label> { AddNewLabel(id + "Help", helpText, language) });
                }

                if (!string.IsNullOrWhiteSpace(viewText))
                {
                    if (addedLabels.ContainsKey("view"))
                        addedLabels["view"].Add(AddNewLabel(id + "View", viewText, language));
                    else
                        addedLabels.Add("view", new List<Label> { AddNewLabel(id + "View", viewText, language) });
                }

                if (!string.IsNullOrWhiteSpace(maintainText))
                {
                    if (addedLabels.ContainsKey("maintain"))
                        addedLabels["maintain"].Add(AddNewLabel(id + "Maintain", maintainText, language));
                    else
                        addedLabels.Add("maintain", new List<Label> { AddNewLabel(id + "Maintain", maintainText, language) });
                }
            }

            return addedLabels;
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
                ok = Helper.Helper.CheckFailed(General.NoLabelSelectedMessage, General.NoLabelSelectedTitle);


            return ok;
        }

        public void Clear()
        {
            All.Clear();
        }
    }
}
